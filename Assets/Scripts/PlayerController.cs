using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public bool TryToRun => ManagersManager.instance.inputManager.Inputs.PlayerGround.Run.ReadValue<float>() > .3f
                            && canRunDueToStaminaExhaustion;
    public bool TryToCrouch => ManagersManager.instance.inputManager.Inputs.PlayerGround.Crouch.ReadValue<float>() > .3f;
    public bool TryToJump => ManagersManager.instance.inputManager.Inputs.PlayerGround.Jump.ReadValue<float>() > .3f;
    public bool TryToMove => ManagersManager.instance.inputManager.Inputs.PlayerGround.Move.ReadValue<Vector2>().magnitude > .3f;
    public bool CanMove => !inventoryController.isOpen;
    public Vector3 LookDir => playerCam.transform.forward;
    public Camera Cam => playerCam;
    public float CurrentSpeed => TryToRun ? runSpeed : TryToCrouch ? crouchSpeed : walkSpeed;

    [Header("References")] 
    [SerializeField] Camera playerCam;
    [SerializeField] CharacterController controller;
    [SerializeField] InventoryController inventoryController;
    [SerializeField] Transform handRoot;

    [Space(10), Header("Mouse Look")] 
    [SerializeField] float sensibility;
    [SerializeField] float xSensibility;
    [SerializeField] float ySensibility;
    [SerializeField] float yMin;
    [SerializeField] float yMax;

    [Space(10), Header("Movements")] 
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;

    [Space(10), Header("Crouch")] 
    [SerializeField] float standHeight;
    [SerializeField] float standCamHeight;
    [SerializeField] float crouchHeight;
    [SerializeField] float crouchCamHeight;
    [SerializeField] float crouchTransitionSpeed;

    [Space(10), Header("Run")] 
    [SerializeField] float normalFOV;
    [SerializeField] float runFOV;
    [SerializeField] float runTransitionSpeed;

    [Space(10), Header("Interactions")] 
    [SerializeField] LayerMask interactionLayers;

    [Space(10), Header("Stamina")] 
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;
    //per seconds
    [SerializeField] private float runStaminaLoseAmount;
    //per jump
    [SerializeField] private float jumpStaminaLoseAmount;
    //per seconds
    [SerializeField] private float regainStaminaAmount;
    [SerializeField] private UnityEngine.UI.Image StaminaBar;
    private bool canRunDueToStaminaExhaustion = true;
    
    private float m_yRot;
    private Vector3 m_moveDir;
    private float m_crouchTransition;
    private float m_runTransition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inventoryController = InventoryController.Instance;

        currentStamina = maxStamina;
    }

    private void Update()
    {
        UpdateStamina();
        
        HandleInventory();

        HandleLook();
        HandleMovement();

        HandleInteractableRayCast();

        HandleJump();
        HandleCrouch();
        HandleRun();

        ApplyMovement();
        
        UpdateStaminaUI();
    }

    
    #region Stamina_ExternalFunctions
    //return == was able to lose stamina
    bool TryLoseStamina(float ActionCostAmount)
    {
        if (HasEnoughStaminaForAction(ActionCostAmount))
        {
            LoseStamina(ActionCostAmount);
            return true;
        }

        return false;
    }
    #endregion
    
    #region Stamina_InternalFunctions
    bool HasEnoughStaminaForAction(float ActionCostAmount)
    {
        if (currentStamina - ActionCostAmount < 0.0f)
        {
            return false;
        }

        return true;
    }

    void UpdateStaminaUI()
    {
        StaminaBar.fillAmount = currentStamina / maxStamina;
    }
    
    void LoseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0.0f)
        {
            currentStamina = 0.0f;
        }
    }

    void GainStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }
    
    void UpdateStamina()
    {
        //to rerun, we need to release the run button before rerunning
        if (!canRunDueToStaminaExhaustion && ManagersManager.instance.inputManager.Inputs.PlayerGround.Run.WasReleasedThisFrame()) 
            canRunDueToStaminaExhaustion = true;
        
        if (TryToRun)
        {
            bool succeed = TryLoseStamina(runStaminaLoseAmount * Time.deltaTime);
            //reached exhaustion, stop running
            if (!succeed)
                canRunDueToStaminaExhaustion = false;
        }
        //only regainStamina when we aren't running
        else
        {
            GainStamina(regainStaminaAmount * Time.deltaTime);
        }
    }
    #endregion
    
    private void ApplyMovement()
    {
        if (inventoryController.isOpen)
        {
            m_moveDir.x = 0;
            m_moveDir.z = 0;
        }

        if (!controller.isGrounded)
            m_moveDir.y -= gravity * Time.deltaTime;

        controller.Move(m_moveDir * Time.deltaTime);
    }

    private void HandleInventory()
    {
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.ToggleInventory.triggered)
            inventoryController.Toggle();
    }

    private void HandleInteractableRayCast()
    {
        if (!CanMove)
            return;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, 3.5f, interactionLayers))
        {
            IInteractable interact = hit.transform.gameObject.GetComponent<IInteractable>();

            interact?.SetInteractionText();

            if (ManagersManager.instance.inputManager.Inputs.PlayerGround.Interact.triggered)
            {
                interact?.DoInteraction();
            }
        }
    }

    private void HandleJump()
    {
        if (!TryToJump || !controller.isGrounded || !CanMove)
            return;
        
        if (!TryLoseStamina(jumpStaminaLoseAmount))
            return;
        
        m_moveDir.y = jumpForce;
    }

    private void HandleMovement()
    {
        if (!CanMove)
            return;

        Vector2 input = ManagersManager.instance.inputManager.Inputs.PlayerGround.Move.ReadValue<Vector2>();

        float moveY = m_moveDir.y;
        m_moveDir = transform.forward * input.y * CurrentSpeed + transform.right * input.x * CurrentSpeed;
        m_moveDir.y = moveY;
    }

    private void HandleLook()
    {
        if (!CanMove)
            return;

        Vector2 input = ManagersManager.instance.inputManager.Inputs.PlayerGround.MouseLook.ReadValue<Vector2>() * sensibility;
        input.x *= xSensibility;
        input.y *= ySensibility;

        m_yRot += input.y;
        m_yRot = Mathf.Clamp(m_yRot, yMin, yMax);

        transform.Rotate(Vector3.up, input.x);
        playerCam.transform.localRotation = Quaternion.AngleAxis(m_yRot, Vector3.left);
    }

    private void HandleCrouch()
    {
        if (!CanMove)
            return;

        m_crouchTransition += (TryToRun ? -1 : TryToCrouch ? 1 : -1) * crouchTransitionSpeed * Time.deltaTime;
        m_crouchTransition = Mathf.Clamp01(m_crouchTransition);

        controller.height = Mathf.Lerp(standHeight, crouchHeight, m_crouchTransition);
        Vector3 pos = playerCam.transform.localPosition;
        pos.y = Mathf.Lerp(standCamHeight, crouchCamHeight, m_crouchTransition);
        playerCam.transform.localPosition = pos;
    }

    private void HandleRun()
    {
        if (!CanMove)
            return;

        m_runTransition += (TryToRun && TryToMove ? 1 : -1) * runTransitionSpeed * Time.deltaTime;
        m_runTransition = Mathf.Clamp01(m_runTransition);

        playerCam.fieldOfView = Mathf.Lerp(normalFOV, runFOV, m_runTransition);
    }

    public void ClearHand()
    {
        foreach (Transform child in handRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetObjectInHand(Item _item)
    {
        if (_item == null || _item.inHandObject == null)
            return;

        GameObject obj = Instantiate(_item.inHandObject, handRoot);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        obj.GetComponent<Holdable>().SetOwner(this);
    }
}
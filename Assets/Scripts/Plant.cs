using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour, IInteractable
{
    [SerializeField] Item item;
    [SerializeField] private Transform lifeBarTransform;

    protected PlayerController m_playerController;
    protected PlayerHealth m_playerHealth;
    private PlantLifebar m_lifebar;

    [SerializeField] protected int pv;
    [SerializeField] protected int maxPv;

    protected bool isStunned = false;
    public bool isWild = true;
    
    [SerializeField] protected float maxCooldown;
    private float currentCooldown = 0.0f;

    [SerializeField] protected float damage;

    [SerializeField] protected float distMin;
    
    
    [SerializeField] public float cdCoin;
    [SerializeField] public int photocoin;

    
    virtual protected void Awake()
    {
        if(item == null)
            Debug.LogError("item is null on plant : " + gameObject.name);
        
        item.currentStackAmount = 1;

        m_playerHealth = FindObjectOfType<PlayerHealth>();
        if (m_playerHealth == null)
            Debug.LogError("Player health is null, couldn't find !");

        m_playerController = FindObjectOfType<PlayerController>();
        if (m_playerController == null)
            Debug.LogError("Player controller is null, couldn't find !");
    }


    protected virtual void DoPlantAction()
    {
        currentCooldown = maxCooldown;
    }

    protected bool CheckCanDoPlantAction()
    {
        return !isStunned && isWild && currentCooldown <= 0.0f;
    }
    
    protected void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0.0f)
            currentCooldown = 0.0f;
        
        if (CheckCanDoPlantAction())
        {
            DoPlantAction();
        }
    }

    void SetStunned()
    {
        if (isStunned)
            return;
        
        isStunned = true;
    }
    
    public void TakeDamage(int _damage)
    {
        pv -= _damage;
        if (pv < 0.0f)
        {
            SetStunned();
        }
        
        m_lifebar.SetFillAmount((float)pv / maxPv);
    }

    private void ShowLifebar()
    {
        if (m_lifebar)
            Destroy(m_lifebar);

        m_lifebar = Instantiate(LifeCanvas.instance.lifeBarPrefab, LifeCanvas.instance.canvas.transform).GetComponent<PlantLifebar>();

        m_lifebar.SetFillAmount((float)pv / maxPv);
        m_lifebar.SetLockState(true);
        m_lifebar.origin = lifeBarTransform;
        m_lifebar.canvas = LifeCanvas.instance.canvas;
    }

    private void FixedUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - LifeCanvas.instance.playerController.transform.position) <= 25)
        {
            if (m_playerController)
                return;

            PlayerController playerController = LifeCanvas.instance.playerController;

            if (playerController)
            {
                m_playerController = playerController;
                ShowLifebar();
            }

            return;
        }

        if (m_playerController && m_lifebar)
        {
            m_lifebar.SetLockState(false);
            m_playerController = null;
        }
    }

    #region Interactible
    public void DoInteraction()
    {
        if (!isWild)
            return;
        Item it = InventoryController.Instance.inventory.AddItem(item);

        if (it == null)
            Destroy(gameObject);
        else
            item = it;
    }

    public void SetInteractionText()
    {
        if (!isWild)
            return;
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to pick {item.displayName}");
    }
    #endregion
}

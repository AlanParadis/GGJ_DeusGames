using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilBag : Holdable
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject soilBallPrefab;

    private void Update()
    {
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.PrimaryAction.triggered)
            Use();
    }

    private void Use()
    {
        animator.SetTrigger("Fire");
    }

    public void ThrowWater()
    {
        GameObject go = Instantiate(soilBallPrefab);
        go.transform.position = transform.position;

        go.GetComponent<SoilBall>().Throw(owner.LookDir * 10f, InventoryController.Instance.hotbar.EquippedItem);

        InventoryController.Instance.hotbar.RemoveOneItem();
    }
}

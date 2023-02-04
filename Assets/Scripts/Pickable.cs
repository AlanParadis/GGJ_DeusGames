using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, IInteractable
{
    [Header("Infos")] 
    [SerializeField] public Item item;
    [SerializeField] int itemAmount;

    private void Awake()
    {
        item.currentStackAmount = itemAmount;
    }

    public void DoInteraction()
    {
        Item it = InventoryController.Instance.inventory.AddItem(item);

        if (it == null)
            Destroy(gameObject);
        else
            item = it;
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        if (item == null)
        {
            ActionUI.Instance.SetText("Unknown item");
            return;
        }

        ActionUI.Instance.SetText($"Press E to pick x{item.currentStackAmount} {item.displayName}");
    }
}

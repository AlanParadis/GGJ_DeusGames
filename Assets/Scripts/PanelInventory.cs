using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelInventory : InventoryPanel
{
    [Header("References")]
    [SerializeField] public Transform grid;
    
    protected List<Slot> m_slots = new List<Slot>();
    private void LoadSlots()
    {
        if (!grid || grid.childCount <= 0)
            return;

        for (int i = 0; i < InventoryController.Instance.hotbar.grid.childCount; i++)
            m_slots.Add(InventoryController.Instance.hotbar.grid.GetChild(i).GetComponent<Slot>());

        for (int i = 0; i < grid.childCount; i++)
            m_slots.Add(grid.GetChild(i).GetComponent<Slot>());
    }

    protected void Awake()
    {
        LoadSlots();
    }

    public Item AddItem(Item item)
    {
        if (!item || m_slots.Count <= 0)
        {
            return item;
        }

        if (item.currentStackAmount <= 0)
        {
            return item;
        }

        int amount = item.currentStackAmount;

        while (amount > 0)
        {
            Slot slotFound = m_slots.FirstOrDefault(
                    slot => slot.GetItem() != null
                    && slot.GetItem().id == item.id
                    && slot.GetItem().maxStackAmount > 1
                    && slot.GetItem().currentStackAmount < slot.GetItem().maxStackAmount);

            int ToAdd = 0;

            if (slotFound != null)
            {
                ToAdd = Mathf.Min(amount, slotFound.GetItem().maxStackAmount - slotFound.GetItem().currentStackAmount);

                slotFound.GetItem().currentStackAmount += ToAdd;

                amount -= ToAdd;

                slotFound.Refresh();
            }
            else
            {
                slotFound = slotFound = m_slots.FirstOrDefault(slot => slot.GetItem() == null);

                if (!slotFound)
                {
                    Item it = Instantiate(item);
                    it.currentStackAmount = amount;

                    return it;
                }

                ToAdd = Mathf.Min(amount, item.maxStackAmount); ;

                item.currentStackAmount = ToAdd;

                slotFound.SetItem(item);

                amount -= ToAdd;

                slotFound.Refresh();
            }
        }

        return null;
    }

    public void StackAllInSlot(Slot _slot)
    {
        if (_slot == null || _slot.GetItem() == null || _slot.GetItem().maxStackAmount <= 1)
            return;

        Item item = _slot.GetItem();

        List<Slot> slots = m_slots.Where(slot => slot != _slot && slot.HasSameItem(_slot)).ToList();
        slots.Sort((a, b) => a.GetItem().currentStackAmount.CompareTo(b.GetItem().currentStackAmount));
        
        bool hasChanged = false;

        foreach (Slot slot in slots)
        {
            int ToAdd = Mathf.Min(_slot.GetItem().maxStackAmount - _slot.GetItem().currentStackAmount, slot.GetItem().currentStackAmount);

            _slot.GetItem().currentStackAmount += ToAdd;
            slot.GetItem().currentStackAmount -= ToAdd;

            slot.Refresh();
            _slot.Refresh();
            
            _slot.onSetItem?.Invoke(_slot);

            hasChanged = true;

            if (item.currentStackAmount == item.maxStackAmount)
                break;
        }

        if (hasChanged)
            _slot.onSetItem?.Invoke(_slot);
    }

    public int GetTotalOfThisItem(Item _item)
    {
        int value = 0;
        foreach (Slot slot in m_slots)
        {
            if (slot.GetItem() != null && slot.GetItem().id == _item.id)
                value += slot.GetItem().currentStackAmount;
        }

        return value;
    }

    public void RemoveItem(string _id, int _amount)
    {
        if (_amount <= 0)
            return;

        List<Slot> slots = m_slots.Where(slot => slot.GetItem() != null && slot.GetItem().id == _id).ToList();
        slots.Sort((a, b) => a.GetItem().currentStackAmount.CompareTo(b.GetItem().currentStackAmount));

        foreach (Slot slot in slots)
        {
            int ToRemove = Mathf.Min(_amount, slot.GetItem().currentStackAmount);

            slot.GetItem().currentStackAmount -= ToRemove;
            _amount -= ToRemove;

            slot.Refresh();

            if (_amount <= 0)
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherable : MonoBehaviour
{
    [System.Serializable]
    public struct AvailableTool
    {
        public Item tool;
        public float efficiency;
    }
    
    [System.Serializable]
    public struct GatherableItem
    {
        public Item resource;
        public int chance;
        public int dropAmount;
    }

    [Header("Tools")]
    [SerializeField] List<AvailableTool> availableTools = new List<AvailableTool>();

    [Header("Gathering")]
    [SerializeField] int life;
    [SerializeField] public List<GatherableItem> itemPool = new List<GatherableItem>();

    [Header("FX")]
    public GameObject breakFX;
    
    public virtual Item Gather(Item tool)
    {
        if (tool == null)
            return null;

        AvailableTool availableTool = availableTools.Find(x => x.tool.id == tool.id);

        if (availableTool.tool == null)
            return null;

        int totalChance = 0;
        foreach (GatherableItem item in itemPool)
            totalChance += item.chance;

        int random = Random.Range(0, totalChance);

        Item it = null;

        int currentChance = 0;
        foreach (GatherableItem item in itemPool)
        {
            currentChance += item.chance;

            if (random <= currentChance)
            {
                int amount = (int)(availableTool.efficiency * item.dropAmount);
                if (amount == 0)
                    amount = 1;
                it = Instantiate(item.resource);
                it.currentStackAmount = amount;
                
                Item rest = InventoryController.Instance.inventory.AddItem(it);
                if (rest != null)
                    Debug.LogError("TODO : Drop rest item");
                
                break;
            }
        }

        life--;

        if (life <= 0)
            Break();

        return it;
    }

    protected void Break()
    {
        Destroy(gameObject);
    }
}

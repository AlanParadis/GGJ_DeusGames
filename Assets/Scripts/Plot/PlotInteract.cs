using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Plot plot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoInteraction()
    {
        throw new System.NotImplementedException();
        // get wich item is on hand
        Item item = InventoryController.Instance.hotbar.GetSelectedItem();
        if (item == null || plot.plotState != PlotState.Empty)
        {
            // if nothing is on hand, open the plot interface
            //plot.Open();
            return;
        }
        if (item.id.ToLower().Contains("plant") || item.id.ToLower().Contains("flower"))
        {
            // if the item is a seed, plant it
            //plot.Plant(item);
            plot.SetPlantMode(item);
        }
        else
        {
            // if the item is not a seed, open the plot interface
            plot.SetFarmMode(item);
        }
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to configure");
    }
}

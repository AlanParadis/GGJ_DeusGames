using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlotInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Plot plot;
    [SerializeField] Canvas typePlotWindow;
    [SerializeField] Canvas addPlantWindow;
    [SerializeField] Canvas addDirtWindow;
    // Start is called before the first frame update
    void Start()
    {
        typePlotWindow.enabled = false;
        addPlantWindow.enabled = false;
        addDirtWindow.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseSlotWindow()
    {
        typePlotWindow.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetParcelle(int _state)
    {
        switch (_state)
        {
            case 0:
                plot.plotState = PlotState.Plant;
                break;
            case 1:
                plot.plotState = PlotState.Farm;
                break;
            default:
                break;
        }
        CloseSlotWindow();
    }


    public void SetPlantMode(Item _item)
    {
        if (InventoryController.Instance.inventory.GetTotalOfThisItem(_item) <= 0)
            return;
        plot.AddPlant(_item);
        //todo enlever la fleur de l'inventaire
        addPlantWindow.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }


    public void SetFarmMode(Item _item)
    {
        addDirtWindow.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void DoInteraction()
    {
        switch (plot.plotState)
        {
            case PlotState.Empty:
                //Open Window for chose Type of plot (plant or Farm)
                typePlotWindow.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
            case PlotState.Plant:
                //Open Window for chose type of plant to harvest
                addPlantWindow.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                break;
            case PlotState.Farm:
                //Open Window for chose type of dirt to harvest
                addDirtWindow.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                break;
            default:
                break;
        }
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to configure");
    }
}

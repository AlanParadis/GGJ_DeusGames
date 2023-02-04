using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlotInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Plot plot;
    [SerializeField] Canvas typePlotWindow;
    [SerializeField] Canvas addPlantWindow;
    [SerializeField] Canvas addDirtWindow;
    [SerializeField] Canvas upgradeWindow;
    
    public List<PlantPlotUpgrades> PlantUpgrades;
    public List<FarmPlotUpgrades> FarmUpgrades;
    
    // Start is called before the first frame update
    void Start()
    {
        typePlotWindow.gameObject.SetActive(false);
        addPlantWindow.gameObject.SetActive(false);
        addDirtWindow.gameObject.SetActive(false);
        upgradeWindow.gameObject.SetActive(false);

        var UpgradeObjects = Resources.LoadAll("PlantUpgrade", typeof(PlantPlotUpgrades));
        foreach (var upgrade in UpgradeObjects)
        {
            PlantUpgrades.Add(upgrade as PlantPlotUpgrades);
        }
        UpgradeObjects = Resources.LoadAll("FarmUpgrade", typeof(FarmPlotUpgrades));
        foreach (var upgrade in UpgradeObjects)
        {
            FarmUpgrades.Add(upgrade as FarmPlotUpgrades);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseWindow()
    {
        addPlantWindow.gameObject.SetActive(false);
        addDirtWindow.gameObject.SetActive(false);
        typePlotWindow.gameObject.SetActive(false);
        upgradeWindow.gameObject.SetActive(false);
        InventoryController.Instance.isOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetParcelle(int _state)
    {
        plot.plotState = (PlotState)_state;
        CloseWindow();
    }


    public void SetPlantMode(Item _item)
    {
        if (InventoryController.Instance.inventory.GetTotalOfThisItem(_item) <= 0)
            return;
        plot.AddPlant(_item);
        //enleve la fleur de l'inventaire
        InventoryController.Instance.inventory.RemoveItem(_item.id, 1);
        CloseWindow();
    }

    public void SetFarmMode(Item _item)
    {
        //todo add dirt to farm here
        CloseWindow();
    }

    public void ClearBtn()
    {
        foreach (var item in plot.plants)
        {
            Destroy(item);
        }
        plot.plants.Clear();
        plot.plantItem = null;
        plot.PlantPrefab = null;
        //todo clear dirt list
        CloseWindow();
    }

    public void DeleteBtn()
    {
        ClearBtn();
        plot.plotState = PlotState.Empty;
        CloseWindow();
    }

    public void DoInteraction()
    {
        InventoryController.Instance.isOpen = true;
        switch (plot.plotState)
        {
            case PlotState.Empty:
                //Open Window for chose Type of plot (plant or Farm)
                typePlotWindow.gameObject.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
            case PlotState.Plant:
                //Open Window for chose type of plant to harvest
                if (plot.PlantPrefab != null)
                {
                    upgradeWindow.gameObject.SetActive(true);
                    upgradeWindow.GetComponent<UpgradeInterface>().SetupButtons(PlantUpgrades.ConvertTo<List<Buyable>>());
                }
                else
                {
                    addPlantWindow.gameObject.SetActive(true);
                }
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                break;
            case PlotState.Farm:
                //Open Window for chose type of dirt to harvest
                if (plot.PlantPrefab != null)
                {
                    upgradeWindow.gameObject.SetActive(true);
                    upgradeWindow.GetComponent<UpgradeInterface>().SetupButtons(FarmUpgrades.ConvertTo<List<Buyable>>());
                }
                else
                {
                    addDirtWindow.gameObject.SetActive(true);
                }
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

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
    [SerializeField] Canvas infoWindow;

    [SerializeField]InfoInterface infoInterface;
    [SerializeField]UpgradeInterface upgradeInterface;

    public List<PlantPlotUpgrades> PlantUpgrades;
    public List<FarmPlotUpgrades> FarmUpgrades;
    
    // Start is called before the first frame update
    void Start()
    {
        typePlotWindow.gameObject.SetActive(false);
        addPlantWindow.gameObject.SetActive(false);
        addDirtWindow.gameObject.SetActive(false);
        upgradeWindow.gameObject.SetActive(false);
        infoWindow.gameObject.SetActive(false);

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

        if (infoInterface == null)
            infoInterface = infoWindow.GetComponent<InfoInterface>();

        if (upgradeInterface == null)
            upgradeInterface = upgradeWindow.GetComponent<UpgradeInterface>();

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
        infoWindow.gameObject.SetActive(false);
        InventoryController.Instance.isOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetParcelle(int _state)
    {
        typePlotWindow.gameObject.SetActive(false);
        plot.plotState = (PlotState)_state;
        if (plot.plotState == PlotState.Plant)
            addPlantWindow.gameObject.SetActive(true);
        else if (plot.plotState == PlotState.Farm)
            addDirtWindow.gameObject.SetActive(true);
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
        if (InventoryController.Instance.inventory.GetTotalOfThisItem(_item) <= 0)
            return;
        plot.AddFarm(_item);
        InventoryController.Instance.inventory.RemoveItem(_item.id, 1);
        CloseWindow();
    }

    public void ClearBtn()
    {
        foreach (var item in plot.plants)
        {
            Destroy(item);
        }
        plot.plants.Clear();
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
                    ShowInfoWindow();
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
                if (plot.NutrimentPrefab != null)
                {
                    ShowInfoWindow();
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

    // coroutine that update in real time the info panel if the panel is active
    public IEnumerator UpdateInfoCoroutine()
    {
        while (infoWindow.gameObject.activeSelf)
        {
            UpdateInfo();
            yield return 0;
        }
    }

    public void ShowInfoWindow()
    {
        infoWindow.gameObject.SetActive(true);
        upgradeWindow.gameObject.SetActive(false);
        StartCoroutine(UpdateInfoCoroutine());
    }

    public void ShowUpgradeWindow()
    {
        if (plot.plotState == PlotState.Plant)
        {
            // convert the list of PlantPlotUpgrades to a list of Buyable
            List<Buyable> buyables = new List<Buyable>();
            foreach (var item in PlantUpgrades)
            {
                buyables.Add(item);
            }
            upgradeInterface.SetupButtons(buyables);
        }
        else
        {
            // convert the list of PlantPlotUpgrades to a list of Buyable
            List<Buyable> buyables = new List<Buyable>();
            foreach (var item in FarmUpgrades)
            {
                buyables.Add(item);
            }
            upgradeInterface.SetupButtons(buyables);
        }
        infoWindow.gameObject.SetActive(false);
        upgradeWindow.gameObject.SetActive(true);
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to configure");
    }

    private void UpdateInfo()
    {
        if (plot.plotState == PlotState.Farm)
        {
            infoInterface.famInfo.SetActive(true);
            infoInterface.plantInfo.SetActive(false);
            infoInterface.f_growRate.text = plot.farmGrowRate.ToString();
            infoInterface.f_productionRate.text = plot.farmProductionRate.ToString();
        }
        else if (plot.plotState == PlotState.Plant)
        {
            infoInterface.famInfo.SetActive(false);
            infoInterface.plantInfo.SetActive(true);
            // grow rate up to 2 decimal
            infoInterface.p_growRate.text =plot.plantGrowRate.ToString("0.000");
            float ratio = DayNightCycle.Instance.GetCurrentTimeInRatio();
            // if ration < 0.5 then it's day, and remap the ratio to 0-1, else it's night so it's 0
            if (ratio < 0.5f)
                ratio = Mathf.InverseLerp(0, 0.5f, ratio);
            else
                ratio = 0;
            infoInterface.p_sunlightAmount.text = ((int)ratio * 10).ToString();
            infoInterface.p_waterAmount.text = ((int)plot.waterAmount).ToString();
            infoInterface.p_waterConsumptionRate.text = plot.waterConsumptionRate.ToString();
            infoInterface.p_photocoinGeneration.text = plot.PhotocoinGeneration.ToString();
            infoInterface.p_soilAmount.text = ((int)plot.soilAmount).ToString();
            infoInterface.p_soilConsumptionRate.text = plot.soilConsumptionRate.ToString();
        }
    }

}

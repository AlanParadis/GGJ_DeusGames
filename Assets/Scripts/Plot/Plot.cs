using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlotState
{
    Empty,
    Plant,
    Farm
}



public class Plot : MonoBehaviour
{
    // std pairs of plant and soils
    public List<Tuple<string, string>> plantSoilPairs = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("FireFlower", "soil_fire"),
        new Tuple<string, string>("IceFlower", "soil_ice"),
        new Tuple<string, string>("LoveFlower", "soil_pink"),
        new Tuple<string, string>("RoseFlower", "soil"),
        new Tuple<string, string>("RadPlant", "soil_rad")
    };
    public PlotState plotState;
    [SerializeField] public PlotInteract plotInteract;
    public Item plantItem;

    [SerializeField] public List<PlantPlotUpgrades> plantPlotUpgrades;
    [SerializeField] public List<FarmPlotUpgrades> farmPlotUpgrades;
    
    public GameObject NutrimentPrefab;
    public GameObject PlantPrefab;
    
    [Header("Plant")]
    [SerializeField] Transform spawnPoint;
    private bool CanAttackPlayer => waterAmount <= 0.0f;
    //stats
    [SerializeField] float plantPhotocoinProdMultiplier = 1.0f; //multiplier, 1 at start
    float photocoinCurrentCooldown;
    public float waterAmount; //0->100
    public float maxWaterAmount = 100.0f;
    [SerializeField] public float waterConsumptionRate; //per seconds
    [SerializeField] public float soilConsumptionRate; // per seconds
    [SerializeField] public float soilAmount; //0->100
    [SerializeField] public string soilID;
    public int PhotocoinGeneration;
    
    //grow
    [SerializeField] public float plantGrowRate;
    [SerializeField] public float plantGrowTime;
    float currentPlantGrowTimer;
    [SerializeField] public int MaxPlantCount;
    public List<GameObject> plants;
    
    [Header("Farm")]
    [SerializeField] public float farmGrowRate;
    [SerializeField] public float farmGrowTime;
    float farmGrowTimer;
    // production rate of 0.1 represent a  10% chance of producing another nutriment
    [SerializeField, Tooltip("production rate of 0.1 represent a  10% chance of producing another nutriment")]
    public float farmProductionRate;
    public int limitNutriment;
    public List<GameObject> nutriments;


    // Start is called before the first frame update
    void Start()
    {
        plotState = PlotState.Empty;
        // initialize to empty list
        plants = new List<GameObject>();
        nutriments = new List<GameObject>();
    }
    public void AddPlant(Item _item)
    {
        //add upgrades
        foreach (PlantPlotUpgrades upgrade in plantPlotUpgrades)
        {
            plantGrowRate += upgrade.growthSpeedModifier;
            plantPhotocoinProdMultiplier += upgrade.photocoinProdModifier;
            waterConsumptionRate += upgrade.waterConsumptionModifier;
            soilConsumptionRate += upgrade.soilConsumptionModifier;

        }
        //set plant
        plantItem = _item;
        PlantPrefab = plantItem.plantGO;

        currentPlantGrowTimer = plantGrowTime;
    }
    
    public void AddFarm(Item _item)
    {
        foreach (FarmPlotUpgrades upgrade in farmPlotUpgrades)
        {
            farmGrowRate += upgrade.nutrimentProductionSpeedModifier;
            farmProductionRate += upgrade.nutrimentProductionModifier;
        }
        plantItem = _item;
        NutrimentPrefab = _item.plantGO;
        farmGrowTimer = farmGrowTime;
    }

    Vector3 SpawnPositionInBound()
    {
        return new Vector3(
            spawnPoint.position.x + Random.insideUnitCircle.x * 0.5f,
            spawnPoint.position.y,
            spawnPoint.position.z + Random.insideUnitCircle.y * 0.5f);
    }

    void PlantGrowUpdate()
    {
        plantGrowRate = Time.deltaTime / (soilAmount / 100);
        currentPlantGrowTimer += plantGrowRate;
        if (currentPlantGrowTimer >= plantGrowTime && waterAmount > 0)
        {
            currentPlantGrowTimer = 0;
            // spawn plant
            if (PlantPrefab != null && plants.Count < MaxPlantCount)
            {
                // spawn plant
                if (PlantPrefab != null)
                {
                    // cube for now
                    Vector3 tempPos = SpawnPositionInBound();
                    GameObject plant = Instantiate(PlantPrefab, tempPos, transform.rotation, transform);
                    plant.GetComponent<Plant>().isWild = false;
                    plant.GetComponent<PlantHostiliti>().enabled = false;
                    plant.transform.Find("Sphere").gameObject.SetActive(false);
                    // scale down
                    //plant.transform.localScale = new Vector3(0.25f, 0.50f, 0.25f);
                    plants.Add(plant);
                }
            }
        }
    }

    void PlantProductionUpdate()
    {
        if (photocoinCurrentCooldown > 0)
            photocoinCurrentCooldown -= Time.deltaTime;
        
        if (photocoinCurrentCooldown <= 0 && plants.Count > 0) 
        {
            GenerateMoula();
            photocoinCurrentCooldown = plants[0].GetComponent<Plant>().cdCoin;
        }
    }
    
    void GenerateMoula()
    {
        float currentTime = DayNightCycle.Instance.GetCurrentTimeInRatio();
        float currentLuminosity = 0.0f;
        if (currentTime > 0.5f)
        {
            currentLuminosity = 0.0f;
        }
        else
        {
            //0 -> 1 (first half of day)
            if(currentTime < 0.25f)
                currentLuminosity = Mathf.Lerp(0.0f, 1.0f, currentTime / 0.25f);
            //1 -> 0 (second half of day)
            else
                currentLuminosity = Mathf.Lerp(1.0f, 0.0f, (currentTime - 0.25f) / 0.25f);
        }

        PhotocoinGeneration = (int)(plants[0].GetComponent<Plant>().photocoin * plants.Count * plantPhotocoinProdMultiplier * currentLuminosity);
        
        for (int i = 0; i < plants.Count; i++)
        {
            GameManager.instance.photocoin += (int)(plants[i].GetComponent<Plant>().photocoin * plantPhotocoinProdMultiplier * currentLuminosity);
        }
    }

    void PlantUpdateWater()
    {
        if (waterAmount <= 0.0f)
            return;
        
        waterAmount -= waterConsumptionRate * Time.deltaTime;
        if (waterAmount <= 0.0f)
        {
            waterAmount = 0.0f;
        }
    }

    void PlantUpdateSoil()
    {
        if (soilAmount <= 0.0f)
            return;
        
        soilAmount -= soilConsumptionRate * Time.deltaTime;
        if (soilAmount <= 0.0f)
        {
            soilAmount = 0.0f;
        }
    }
    
    void PlantUpdate()
    {
        PlantGrowUpdate();
        PlantProductionUpdate();
        PlantUpdateWater();
        PlantUpdateSoil();
    }


    void FarmUpdate()
    {
        farmGrowTimer += Time.deltaTime * farmGrowRate;
        if (farmGrowTimer >= farmGrowTime)
        {
            farmGrowTimer = 0;
            int nutrimenCount = Mathf.Max((int)(farmProductionRate * 10), 1);

            for (int i = 0; i < nutrimenCount && nutriments.Count < limitNutriment; i++)
            {
                // spawn nutriment
                // place nutriment random within plot bounds
                Vector3 tempPos = SpawnPositionInBound();

                // cube for now
                GameObject nutriment = Instantiate(NutrimentPrefab, tempPos, transform.rotation, transform);
                // scale down
                //nutriment.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
                
                nutriments.Add(nutriment);
            }
        }

        //check for null reference in nutriments and remove from list if null
        for (int i = 0; i < nutriments.Count; i++)
        {
            if (nutriments[i] == null)
            {
                nutriments.RemoveAt(i);
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (plotState)
        {
            case PlotState.Empty:
                break;
            case PlotState.Plant:
                PlantUpdate();
                break;
            case PlotState.Farm:
                FarmUpdate();
                break;
            default:
                break;
        }
    }

    public void OnWaterBallEnter(WaterBall waterBall)
    {
        // if plant mode
        if (plotState != PlotState.Plant)
        {
            return;
        }
        waterAmount += 20;
        if (waterAmount > 100)
            waterAmount = 100;
        Destroy(waterBall.gameObject);
    }

    public void OnSoilBallEnter(SoilBall soilBall)
    {
        // if plant mode
        if (plotState != PlotState.Plant)
        {
            return;
        }
        // base on plant item id, check is soilbag item id correspond in the key pair list
        foreach (var plantsoilpair in plantSoilPairs)
        {
            if (plantItem.id == plantsoilpair.Item1)
            {
                // if soilbag item id correspond
                if (soilBall.item.id == plantsoilpair.Item2)
                {
                    soilAmount += 20;
                    if (soilAmount > 100)
                        soilAmount = 100;
                    Destroy(soilBall.gameObject);
                    break;
                }
            }
        }
    }

}

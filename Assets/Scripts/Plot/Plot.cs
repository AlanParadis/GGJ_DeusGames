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
    private float waterAmount; //0->100
    private float maxWaterAmount = 100.0f;
    [SerializeField] private float waterConsumptionRate; //per seconds
    [SerializeField] private float soilAmount; //0->100
    
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
        }
        //set plant
        plantItem = _item;
        PlantPrefab = plantItem.plantGO;
    }
    public void AddFarm(Item _item)
    {
        foreach (FarmPlotUpgrades upgrade in farmPlotUpgrades)
        {
            farmGrowRate += upgrade.nutrimentProductionSpeedModifier;
            farmProductionRate += upgrade.nutrimentProductionModifier;
        }
        plantItem = _item;
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
        currentPlantGrowTimer += Time.deltaTime;
        if (currentPlantGrowTimer >= plantGrowRate)
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
                    // scale down
                    plant.transform.localScale = new Vector3(0.25f, 0.50f, 0.25f);
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
    
    void PlantUpdate()
    {
        PlantGrowUpdate();
        PlantProductionUpdate();
        PlantUpdateWater();
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
                Vector3 spawnPosition = SpawnPositionInBound();

                if (NutrimentPrefab == null)
                {
                    // cube for now
                    GameObject nutriment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    nutriment.transform.position = spawnPosition;
                    // scale down
                    nutriment.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
                    
                    nutriments.Add(nutriment);
                    
                    Destroy(nutriment, 10);
                }
            }
            // spawn nutriment
            if (NutrimentPrefab == null)
            {
                // cube for now
                //GameObject nutriment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // scale down
                //nutriment.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
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
}

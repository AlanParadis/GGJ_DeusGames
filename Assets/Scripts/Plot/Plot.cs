using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public enum PlotState
{
    Empty,
    Plant,
    Farm
}

public class Plot : MonoBehaviour
{
    [SerializeField] public PlotInteract plotInteract;

    [SerializeField] public List<PlantPlotUpgrades> plotUpgrades;
    [SerializeField] public List<FarmPlotUpgrades> farmPlotUpgrades;

    public PlotState plotState;

    [SerializeField]public GameObject NutrimentPrefab;
    [SerializeField]public GameObject PlantPrefab;

    public Item item;

    [Header("Farm")]
    [SerializeField] public float farmGrowRate;
    [SerializeField] public float farmGrowTime;
    float farmGrowTimer;
    // production rate of 0.1 represent a  10% chance of producing another nutriment
    [SerializeField, Tooltip("production rate of 0.1 represent a  10% chance of producing another nutriment")]
    public float farmProductionRate;
    public int limitNutriment;
    public List<GameObject> nutriments;

    [Header("Plant")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] public float plantGrowRate;
    [SerializeField] float plantProductionRate;
    [SerializeField] public float plantGrowTime;
    float plantGrowTimer;
    [SerializeField] public int limitPlant;
    float cdCoin;
    public List<GameObject> plants;

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
        foreach (PlantPlotUpgrades upgrade in plotUpgrades)
        {
            plantGrowRate = upgrade.growthSpeedModifier;
            plantProductionRate = upgrade.plantProductionModifier;
        }
        item = _item;
        PlantPrefab = item.plantGO;
    }
    public void AddFarm(Item _item)
    {
        foreach (FarmPlotUpgrades upgrade in farmPlotUpgrades)
        {
            farmGrowRate += upgrade.nutrimentProductionSpeedModifier;
            farmProductionRate += upgrade.nutrimentProductionModifier;
        }
        item = _item;
    }

    Vector3 SpawnPositionInBound()
    {
        return new Vector3(
            spawnPoint.position.x + Random.insideUnitCircle.x * 0.5f,
            spawnPoint.position.y,
            spawnPoint.position.z + Random.insideUnitCircle.y * 0.5f);
    }

    void PlantUpdate()
    {
        if (cdCoin > 0)
            cdCoin -= Time.deltaTime;
        plantGrowTimer += Time.deltaTime;
        if (plantGrowTimer >= plantGrowRate)
        {
            plantGrowTimer = 0;
            // spawn plant
            if (PlantPrefab != null && plants.Count < limitPlant)
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
        if (cdCoin <= 0 && plants.Count > 0) 
        {
            GenerateMoula();
            cdCoin = plants[0].GetComponent<Plant>().cdCoin;
        }
    }

    void GenerateMoula()
    {
        for (int i = 0; i < plants.Count; i++)
        {
            GameManager.instance.photocoin += plants[i].GetComponent<Plant>().photocoin;
        }
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

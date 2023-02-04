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
    float farmGrowTimer;
    // production rate of 0.1 represent a  10% chance of producing another nutriment
    [SerializeField] public float farmProductionRate;

    [Header("Plant")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] public float plantGrowRate;
    float plantGrowTimer;

    [SerializeField] public int limitPlant;
    int actualPlant;
    public List<GameObject> plants;

    // Start is called before the first frame update
    void Start()
    {
        plotState = PlotState.Empty;
        actualPlant = 0;
    }
    public void AddPlant(Item _item)
    {
        foreach (PlantPlotUpgrades upgrade in plotUpgrades)
        {
            plantGrowRate = 1;
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

    void PlantUpdate()
    {
        plantGrowTimer += Time.deltaTime;
        if (plantGrowTimer >= plantGrowRate)
        {
            plantGrowTimer = 0;

            // spawn plant
            if (PlantPrefab != null && actualPlant < limitPlant)
            {
                actualPlant++;
                // cube for now
                Vector3 tempPos = new Vector3(spawnPoint.position.x + Random.insideUnitCircle.x * 0.5f, spawnPoint.position.y, spawnPoint.position.z + Random.insideUnitCircle.y * 0.5f);
                GameObject plant = Instantiate(PlantPrefab, tempPos, transform.rotation, transform);
                plant.GetComponent<PlantScript>().isSauvage = false;
                // scale down
                plant.transform.localScale = new Vector3(0.25f, 0.50f, 0.25f);
                plants.Add(plant);
            }

        }
    }

    void FarmUpdate()
    {
        farmGrowTimer += Time.deltaTime;
        if (farmGrowTimer >= farmGrowRate)
        {
            farmGrowTimer = 0;

            int nutrimenCount = Mathf.Max((int)(farmProductionRate * 10), 1);

            for (int i = 0; i < nutrimenCount; i++)
            {
                Collider[] colliders = Array.Empty<Collider>();
                while (colliders.Length > 0)
                {
                    // spawn nutriment
                    // place nutriment random within plot bounds
                    Vector3 randomPos = new Vector3(Random.Range(-transform.localScale.x / 2.0f, transform.localScale.x / 2.0f), 0, Random.Range(-transform.localScale.z / 2.0f, transform.localScale.z / 2.0f));
                    Vector3 spawnPos = randomPos + transform.localScale.y / 1.95f * Vector3.up;

                    // check if position collides with anything
                    colliders = Physics.OverlapSphere(spawnPos, 0.25f);
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

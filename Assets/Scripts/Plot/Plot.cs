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
    [SerializeField] public PlotInteract plotInteract;

    [SerializeField] List<PlantPlotUpgrades> plotUpgrades;
    [SerializeField] List<FarmPlotUpgrades> farmPlotUpgrades;

    public PlotState plotState;

    [SerializeField]GameObject NutrimentPrefab;
    [SerializeField]GameObject PlantPrefab;

    Item item;

    [Header("Farm")]
    [SerializeField] float farmGrowRate;
    float farmGrowTimer;
    // production rate of 0.1 represent a  10% chance of producing another nutriment
    [SerializeField] float farmProductionRate;

    [Header("Plant")]
    [SerializeField] float plantGrowRate;
    float plantGrowTimer;


    // Start is called before the first frame update
    void Start()
    {
        plotState = PlotState.Empty;
        SetFarmMode(null);
    }
    
    public void SetPlantMode(Item _item)
    {
        plotState = PlotState.Plant;
        foreach (PlantPlotUpgrades upgrade in plotUpgrades)
        {
            plantGrowRate = 1;
        }
        item = _item;
        PlantPrefab = item.onFloorObject;
    }

    public void SetFarmMode(Item _item)
    {
        plotState = PlotState.Farm;
        foreach (FarmPlotUpgrades upgrade in farmPlotUpgrades)
        {
            farmGrowRate += upgrade.nutrimentProductionSpeedModifier;
            farmProductionRate += upgrade.nutrimentProductionModifier;
        }
        item = _item;
    }

    void PlantUdate()
    {
        plantGrowTimer += Time.deltaTime;
        if (plantGrowTimer >= plantGrowRate)
        {
            plantGrowTimer = 0;

            // spawn plant
            if (PlantPrefab == null)
            {
                // cube for now
                GameObject plant = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // scale down
                plant.transform.localScale = new Vector3(0.25f, 0.50f, 0.25f);
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
                GameObject nutriment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // scale down
                nutriment.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
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
                PlantUdate();
                break;
            case PlotState.Farm:
                FarmUpdate();
                break;
            default:
                break;
        }
    }
}

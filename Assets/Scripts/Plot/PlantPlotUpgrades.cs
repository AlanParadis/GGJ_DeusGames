using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlantPlotUpgrade", menuName = "Scriptables/PlantPlotUpgrades", order = 1), System.Serializable]
public class PlantPlotUpgrades : Buyable
{
    [Header("Plants")]
    // plats modifiers
    public float waterConsumptionModifier;
    public float soilConsumptionModifier;
    public float photocoinProdModifier;
    public float growthSpeedModifier;

}

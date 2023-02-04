using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlantPlotUpgrade", menuName = "Scriptables/PlantPlotUpgrades", order = 1), System.Serializable]
public class PlantPlotUpgrades : Buyable
{
    // plats modifiers
    public float waterConsumptionModifier;
    public float nutrimentConsumptionModifier;
    public float growthSpeedModifier;
    public float plantProductionModifier;
    public float energyProductionModifier;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FarmPlotUpgrade", menuName = "Scriptables/FarmPlotUpgrades", order = 1), System.Serializable]
public class FarmPlotUpgrades : Buyable
{
    // nutriment modifiers
    public float nutrimentProductionModifier;
    public float nutrimentProductionSpeedModifier;

}

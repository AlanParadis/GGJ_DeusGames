using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyable : ScriptableObject
{
    [Header("Price")]
    public int Price;
    public bool Purchased;
    
    [Header("Interface")]
    public Sprite icon;
    public string displayName;
    public string description;
}

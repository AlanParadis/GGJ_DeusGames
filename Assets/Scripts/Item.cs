using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Item", fileName = "New Item"), System.Serializable]
public class Item : ScriptableObject
{
    [Header("General")] 
    public string id;
    public string displayName;
    public string description;
    public Sprite icon;
    public string data;
    public GameObject inHandObject;
    public GameObject onFloorObject;
    public GameObject plantGO;

    [Header("Stack")]
    public int maxStackAmount;
    public int currentStackAmount;

    [Header("Durability")]
    public float maxDurability;
    public float currentDurability;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoInterface : MonoBehaviour
{
    public GameObject famInfo;
    public GameObject plantInfo;

    [Header("Farm")]
    [SerializeField] public TextMeshProUGUI f_growRate;
    [SerializeField] public TextMeshProUGUI f_productionRate;

    [Header("Plant")]
    [SerializeField] public TextMeshProUGUI p_growRate;
    [SerializeField] public TextMeshProUGUI p_waterAmount;
    [SerializeField] public TextMeshProUGUI p_waterConsumptionRate;
    [SerializeField] public TextMeshProUGUI p_soilAmount;
    [SerializeField] public TextMeshProUGUI p_soilConsumptionRate;
    [SerializeField] public TextMeshProUGUI p_sunlightAmount;
    [SerializeField] public TextMeshProUGUI p_photocoinGeneration;


    
}

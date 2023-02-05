using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDesc;

    private void Start()
    {
        ClearInfo();
    }

    public void SetItemInfo(Item _item)
    {
        itemName.text = _item.displayName;
        itemDesc.text = _item.description;
    }

    public void ClearInfo()
    {
        itemName.text = "";
        itemDesc.text = "";
    }
}

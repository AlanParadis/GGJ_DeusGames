using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeInterface : MonoBehaviour
{

    //upgrade name
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDesc;
    [SerializeField] private Image upgradeImage;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private Transform upgradeButtonsRoot;
    [SerializeField] GameObject UpgradeButton;

    public void UpdateInfoInterface(Buyable _buyable)
    {
        upgradeName.text = _buyable.name;
        upgradeDesc.text = _buyable.description;
        upgradeImage.sprite = _buyable.icon;

        if (_buyable.Purchased)
        {
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Purchased";
        }
        else
        {
            upgradeButton.onClick.AddListener(() =>
            {
                if (GameManager.instance.photocoin >= _buyable.Price)
                {
                    GameManager.instance.photocoin -= _buyable.Price;
                    _buyable.Purchased = true;
                }
            });
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = _buyable.Price.ToString();
        }
    }

    public void SetupButtons(List<Buyable> _buyables)
    {
        for(int i=0; i < _buyables.Count; i++)
        {
            Buyable buyable = _buyables[i];
            GameObject buttonGO = Instantiate(UpgradeButton, upgradeButtonsRoot);
            buttonGO.GetComponentInChildren<Image>().sprite = buyable.icon;
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = buyable.displayName;
            Button button = buttonGO.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                UpdateInfoInterface(buyable);
            });
            if (i == 0)
                button.onClick.Invoke();
        }
    }
    
}

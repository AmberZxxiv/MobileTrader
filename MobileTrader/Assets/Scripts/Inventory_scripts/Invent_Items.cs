using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Invent_Items : MonoBehaviour
{// script en cada prefab de items de inventario
    public Image icon;
    public Item_Data itemData;
    public TMP_Text amountText;
    public int Amount { get; private set; }


    public void Setup(Item_Data item, int amount)
    {
        // configuro el prefab con la info de su ScripItem
        itemData = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        Amount = amount;
        UpdateAmount();
        gameObject.SetActive(true);
    }
    public void AddAmount(int value)
    {
        Amount += value;
        UpdateAmount();
    }

    public void DecreaseAmount(int value)
    {
        Amount -= value;
        if (Amount < 0) Amount = 0;
        UpdateAmount();
    }

    public void UpdateAmount()
    {
        amountText.text = "x" + Amount.ToString();
    }

    public void SellItem()
    {
        if (Zones_General._ZG.currentMenu == Zones_General.CurrentMenu.Merchant)
        { Invent_Control._IC.CompleteSell(itemData); }
    }
}

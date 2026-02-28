using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item_Slots : MonoBehaviour
{// script en cada prefab de items de inventario
    public Image icon;
    public TMP_Text amountText;
    public Item_Data itemData;

    public void Setup(Item_Data item, int amount)
    {
        itemData = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        amountText.text = amount.ToString();
        gameObject.SetActive(true);
    }
    public void SellItem()
    {
        if (Scroll_Control._SC.currentMenu == Scroll_Control.CurrentMenu.Merchant)
        Inventory_Manager._IM.CompleteSell(itemData);
    }
    public void UpdateAmount(int amount)
    {
        amountText.text = "x"+amount.ToString();
    }
}

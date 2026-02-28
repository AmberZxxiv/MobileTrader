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
        // configuro el prefab con la info de su ScripItem
        itemData = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        amountText.text = amount.ToString();
        gameObject.SetActive(true);
    }
    public void SellItem() // si estoy en Merch, vendo al tapear
    {
        if (Scroll_Control._SC.currentMenu == Scroll_Control.CurrentMenu.Merchant)
        Inventory_Manager._IM.CompleteSell(itemData);
    }
    public void UpdateAmount(int amount) // actualizo el numero del item
    { amountText.text = "x"+amount.ToString();}
}

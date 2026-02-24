using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{// script canvas porque tiene que estar en algun lao
    public static Inventory_Manager Instance; // creo SINGLE

    public Item_Slots itemPrefab; // prefab del item
    public Transform inventSlots; // panel donde se van a instanciar
    Dictionary<Item_Data, int> inventory = new Dictionary<Item_Data, int>(); //controlo las cantidades
    Dictionary<Item_Data, Item_Slots> slots = new Dictionary<Item_Data, Item_Slots>(); //controlo los items

    void Awake() // single cosas
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(Item_Data item)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += item.amount;
        else inventory[item] = item.amount;

        if (slots.ContainsKey(item))
        { slots[item].UpdateAmount(inventory[item]); }
        else
        {
            Item_Slots newSlot = Instantiate(itemPrefab, inventSlots);
            newSlot.Setup(item, inventory[item]);
            slots[item] = newSlot;
        }
    }
}

using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{// script en el canvas porque si
    public static Inventory_Manager Instance; // creo SINGLE

    public Item_Slots slotPrefab; // prefab del slot
    public Transform content; // panel donde se van a instanciar
    private Dictionary<Item_Data, int> inventory = new Dictionary<Item_Data, int>();
    private Dictionary<Item_Data, Item_Slots> slots = new Dictionary<Item_Data, Item_Slots>();

    void Awake() // Single cosas
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(Item_Data item)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += item.amount;
        else
            inventory[item] = item.amount;

        if (slots.ContainsKey(item))
        {
            slots[item].UpdateAmount(inventory[item]);
        }
        else
        {
            Item_Slots newSlot = Instantiate(slotPrefab, content);
            newSlot.Setup(item, inventory[item]);
            slots[item] = newSlot;
        }
    }
}

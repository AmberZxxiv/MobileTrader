using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{// script canvas porque tiene que estar en algun lao
    public static Inventory_Manager _IM; //SINGLE
    public Scroll_Control _SC; //pillo SINGLE del SC

    public Item_Slots itemPrefab; // prefab del item
    public Transform inventSlots; // panel donde se van a instanciar
    public int maxItems; // limito el inventario
    Dictionary<Item_Data, int> inventory = new Dictionary<Item_Data, int>(); //controlo las cantidades
    Dictionary<Item_Data, Item_Slots> slots = new Dictionary<Item_Data, Item_Slots>(); //controlo los items

    void Awake() // SINGLE cosas
    {
        if (_IM == null) _IM = this;
        else Destroy(gameObject);
        if (_SC == null) { _SC = Scroll_Control._SC; }
    }

    public void AddItem(Item_Data item)
    {
        if (inventory.ContainsKey(item))
        { inventory[item] += item.amount; }
        else inventory[item] = item.amount;

        if (slots.ContainsKey(item))
        { slots[item].UpdateAmount(inventory[item]); }
        else
        {
            if (slots.Count >= maxItems)
            { print("Inventario lleno"); return;}
            Item_Slots newSlot = Instantiate(itemPrefab, inventSlots);
            newSlot.Setup(item, inventory[item]);
            slots[item] = newSlot;
        }
    }
    public void BuyPlant()
    {
        // busco parcelas CROPS
        foreach (Parcel_Limiter parcel in _SC.cropsParcels)
        {
                // compruebo las plantas en las parcelas
                bool added = parcel.ToAddPlant();
                if (!added)
                { print("Crop llena, siguiente"); continue;}
                else
                { print("Planta añadida"); break;}
        }
    }
    public void BuyAnimal()
    {
        // busco parcelas FARM
        foreach (Parcel_Limiter parcel in _SC.farmParcels)
        {
            // compruebo las plantas en las parcelas
            bool added = parcel.ToAddAnimal();
            if (!added)
            { print("Farm llena, siguiente"); continue; }
            else
            { print("Animal añadido"); break; }
        }
    }
}

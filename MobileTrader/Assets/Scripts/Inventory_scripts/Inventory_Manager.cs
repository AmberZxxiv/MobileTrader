using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{// script canvas porque tiene que estar en algun lao
    public static Inventory_Manager _IM; // declaro SINGLE
    public Scroll_Control _SC; //pillo SINGLE del SC

    public Item_Slots itemPrefab; // prefab del item
    public Transform inventSlots; // panel donde se van a instanciar
    public int maxItems; // limito el inventario
    Dictionary<Item_Data, int> inventory = new Dictionary<Item_Data, int>(); //controlo las cantidades
    Dictionary<Item_Data, Item_Slots> slots = new Dictionary<Item_Data, Item_Slots>(); //controlo los items

    void Awake() // declaro SINGLE
    { if (_IM == null) _IM = this; else Destroy(gameObject);}
    private void Start() //pillo SINGLE del SC
    { if (_SC == null) { _SC = Scroll_Control._SC; } }

    public void AddItem(Item_Data item) // animals y plants al tapear añaden recursos
    {
        if (inventory.ContainsKey(item))
        { inventory[item] += item.collectAmount; }
        else inventory[item] = item.collectAmount;

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
    public void CompleteSell(Item_Data item) // recursos al tapear en merch se venden
    {
        if (!inventory.ContainsKey(item)) return;

        inventory[item]--;
       _SC.UpdateMoney(item.sellPrice);

        if (inventory[item] <= 0)
        {
            Destroy(slots[item].gameObject);
            slots.Remove(item);
            inventory.Remove(item);
        }
        else
        { slots[item].UpdateAmount(inventory[item]);}
    }

    public void BuyItem(Item_Data item)
    {
        if (_SC.moneyCount < item.buyPrice) return;

        Parcel_Limiter parcel = null;

        if (item.type == Item_Data.ItemType.Plant)
        {
            parcel = GetAvailableCropParcel();
            if (parcel == null) { print("No hay parcelas para plantas"); return; }
            if (parcel.ToAddPlant(item.prefab)) { _SC.UpdateMoney(-item.buyPrice); print(item.itemName + " añadida"); }
        }
        if (item.type == Item_Data.ItemType.Animal)
        {
            parcel = GetAvailableFarmParcel();
            if (parcel == null) { print("No hay parcelas para animales"); return; }
            if (parcel.ToAddAnimal(item.prefab)) { _SC.UpdateMoney(-item.buyPrice); print(item.itemName + " añadido"); }
        }
        if (item.type == Item_Data.ItemType.Pet)
        {
            parcel = GetAvailableBaseParcel();
            if (parcel == null) { print("No hay base disponible"); return; }
            if (parcel.ToAddPet(item.prefab)) { _SC.UpdateMoney(-item.buyPrice); print(item.itemName + " añadido"); }
        }
    }
    Parcel_Limiter GetAvailableCropParcel()
    {
        foreach (Parcel_Limiter parcel in _SC.cropsParcels)
        { if (parcel.HasSpaceForPlant()) return parcel;}
        return null;
    }

    Parcel_Limiter GetAvailableFarmParcel()
    {
        foreach (Parcel_Limiter parcel in _SC.farmParcels)
        { if (parcel.HasSpaceForAnimal()) return parcel;}
        return null;
    }
    Parcel_Limiter GetAvailableBaseParcel()
    {
        foreach (Parcel_Limiter parcel in _SC.baseParcels)
        { if (parcel.HasSpaceForPets()) return parcel;}
        return null;
    }
    public void BuyParcBase()
    { 
        if (_SC.moneyCount > 0)
        {
            _SC.UpdateMoney(-1);
            _SC.ToAddBase();
        }
    }
    public void BuyParcCrops()
    {
        if (_SC.moneyCount > 0)
        {
            _SC.UpdateMoney(-1);
            _SC.ToAddCrops();
        }
    }
    public void BuyParcFarm()
    {
        if (_SC.moneyCount > 0)
        {
            _SC.UpdateMoney(-1);
            _SC.ToAddFarm();
        }
    }
}

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

    public void BuyItem(Item_Data item) // llamo desde Buy_Buttons
    {
        // no compro si no tengo dinero o espacio
        if (_SC.moneyCount < item.buyPrice) return;
        Parcel_Limiter parcel = GetAvailableParcel(item.type);
        if (parcel == null) { print("no queda espacio"); return; }
        // añado el recurso a la parcela
        if (parcel.ToAddResource(item.prefab, GetResourceType(item.type)))
        {
            _SC.UpdateMoney(-item.buyPrice);
            print(item.itemName + " añadido");
        }
    }
    private Parcel_Limiter.ResourceType GetResourceType(Item_Data.ItemType type)
    { // selecciono el tipo del ScripItem
        switch (type)
        {
            case Item_Data.ItemType.Plant: return Parcel_Limiter.ResourceType.Plant;
            case Item_Data.ItemType.Animal: return Parcel_Limiter.ResourceType.Animal;
            case Item_Data.ItemType.Pet: return Parcel_Limiter.ResourceType.Pet;
            default: throw new System.Exception("Tipo de item desconocido");
        }
    }
    private Parcel_Limiter GetAvailableParcel(Item_Data.ItemType type)
    { // busco parcelas en las listas correspondientes
        switch (type)
        {
            case Item_Data.ItemType.Plant:
                foreach (var parcel in _SC.cropsParcels)
                if (parcel.HasSpace(Parcel_Limiter.ResourceType.Plant))
                return parcel; break;
            case Item_Data.ItemType.Animal:
                foreach (var parcel in _SC.farmParcels)
                if (parcel.HasSpace(Parcel_Limiter.ResourceType.Animal))
                return parcel; break;
            case Item_Data.ItemType.Pet:
                foreach (var parcel in _SC.baseParcels) 
                if (parcel.HasSpace(Parcel_Limiter.ResourceType.Pet))
                return parcel; break;
        } return null;
    }
    // compra de parcelas desde Inventario
    public void BuyParcBase()
    { BuyParcel(_SC.ToAddBase); }
    public void BuyParcCrops()
    { BuyParcel(_SC.ToAddCrops); }
    public void BuyParcFarm()
    { BuyParcel(_SC.ToAddFarm); }
    private void BuyParcel(System.Action addAction)
    {
        if (_SC.moneyCount <= 0) return;
        _SC.UpdateMoney(-1);
        addAction();
    }
}

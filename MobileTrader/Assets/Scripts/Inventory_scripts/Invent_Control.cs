using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Invent_Control : MonoBehaviour
{// script canvas porque tiene que estar en algun lao
    public static Invent_Control _IC; // declaro SINGLE
    public Zones_General _ZG; //pillo SINGLE del ZG

    public Invent_Items itemPrefab; // prefab del item
    public Transform pagesParent; // empty padre de todos los invetnarios
    public GameObject inventPagePref; // prefab de las paginas
    private List<Invent_Pages> pages = new List<Invent_Pages>(); //lista de paginas
    public int currentPage = 0;

    void Awake() // declaro SINGLE
    { if (_IC == null) _IC = this; else Destroy(gameObject);}
    private void Start() 
    {
        //pillo SINGLE del ZG
        if (_ZG == null) { _ZG = Zones_General._ZG; }
        // genero la primera pag de inventario y la muestro
        CreateNewPage();
        ShowPage(0);
    }

    void CreateNewPage()
    {
        GameObject pageObj = Instantiate(inventPagePref, pagesParent);
        Invent_Pages page = pageObj.GetComponent<Invent_Pages>();
        pageObj.SetActive(false);
        pages.Add(page);
    }
    public void ShowPage(int index)
    {
        if (pages.Count == 0) return;
        if (index < 0 || index >= pages.Count) return;
        // limpio todas
        foreach (var pag in pages) pag.gameObject.SetActive(false);
        // activo la actual
        pages[index].gameObject.SetActive(true);
        currentPage = index;
    }
    public void NextPage()
    { ShowPage((currentPage + 1) % pages.Count); }

    public void PreviousPage()
    { ShowPage((currentPage - 1 + pages.Count) % pages.Count); }
    public void BuyInventoryPage()
    {
        int price = 1;
        if (Zones_General._ZG.moneyCount >= price)
        {
            Zones_General._ZG.UpdateMoney(-price);
            CreateNewPage();
            print("new wagon added");
        }
        else print("no hay dinero");
    }

    public void AddItem(Item_Data item) // animals y plants al tapear añaden recursos
    {
        foreach (var page in pages)
        { if (page.TryAddItem(item, itemPrefab)) return;}
        print("inventario lleno");
    }
    public void CompleteSell(Item_Data item) // ivent items en merch se venden
    {
        foreach (var page in pages)
        {
            if (page.TryRemoveItem(item))
            { _ZG.UpdateMoney(item.sellPrice); return;}
        }
    }

    public void BuyItem(Item_Data item) // llamo desde Buy_Buttons
    {
        // no compro si no tengo dinero o espacio
        if (_ZG.moneyCount < item.buyPrice) return;
        Parcel_Individual parcel = GetAvailableParcel(item.type);
        if (parcel == null) { print("no queda espacio"); return; }
        // añado el recurso a la parcela
        if (parcel.ToAddResource(item.prefab, GetResourceType(item.type)))
        {
            _ZG.UpdateMoney(-item.buyPrice);
            print(item.itemName + " añadido");
        }
    }
    private Parcel_Individual.ResourceType GetResourceType(Item_Data.ItemType type)
    { // selecciono el tipo del ScripItem
        switch (type)
        {
            case Item_Data.ItemType.Plant: return Parcel_Individual.ResourceType.Plant;
            case Item_Data.ItemType.Animal: return Parcel_Individual.ResourceType.Animal;
            case Item_Data.ItemType.Pet: return Parcel_Individual.ResourceType.Pet;
            default: throw new System.Exception("Tipo de item desconocido");
        }
    }
    private Parcel_Individual GetAvailableParcel(Item_Data.ItemType type)
    { // busco parcelas en las listas correspondientes
        switch (type)
        {
            case Item_Data.ItemType.Plant:
                foreach (var parcel in _ZG.cropsParcels)
                if (parcel.HasSpace(Parcel_Individual.ResourceType.Plant))
                return parcel; break;
            case Item_Data.ItemType.Animal:
                foreach (var parcel in _ZG.farmParcels)
                if (parcel.HasSpace(Parcel_Individual.ResourceType.Animal))
                return parcel; break;
            case Item_Data.ItemType.Pet:
                foreach (var parcel in _ZG.baseParcels) 
                if (parcel.HasSpace(Parcel_Individual.ResourceType.Pet))
                return parcel; break;
        } return null;
    }
    // compra de parcelas desde Inventario
    public void BuyParcBase()
    { BuyParcel(_ZG.ToAddBase); }
    public void BuyParcCrops()
    { BuyParcel(_ZG.ToAddCrops); }
    public void BuyParcFarm()
    { BuyParcel(_ZG.ToAddFarm); }
    private void BuyParcel(System.Action addAction)
    {
        if (_ZG.moneyCount <= 0) return;
        _ZG.UpdateMoney(-1);
        addAction();
    }
}

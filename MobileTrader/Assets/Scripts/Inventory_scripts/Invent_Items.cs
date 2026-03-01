using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class Invent_Items : MonoBehaviour,
IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{// script en cada prefab de item de inventario
    public int Amount { get; private set; }
    public Image icon;
    public Item_Data itemData;
    public TMP_Text amountText;

    Transform _originParent;
    int _originIndex;
    CanvasGroup _canvasGroup;
    bool _canDrag = false;
    bool _dropOnItem = false;

    void Awake() // le doy un grupo al item
    { _canvasGroup = gameObject.AddComponent<CanvasGroup>(); }

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

    public void AddAmount(int value) //sumo y actualizo
    { Amount += value; UpdateAmount(); }
    public void DecreaseAmount(int value) //resto y actualizo
    {
        Amount -= value;
        if (Amount < 0) Amount = 0;
        UpdateAmount();
    }
    public void UpdateAmount() // actualizo cantidad de items
    { amountText.text = "x" + Amount.ToString(); }

    public void SellItem() // si estoy en merch, al tapear vendo
    {
        if (Zones_General._ZG.currentMenu == Zones_General.CurrentMenu.Merchant)
        { Invent_Control._IC.CompleteSell(itemData); }
    }

    bool IsWagonMenu() // compruebo si estoy en wagon
    {return Zones_General._ZG.currentMenu == Zones_General.CurrentMenu.Wagon;}

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsWagonMenu()) return;
        // guardo los datos de su origen y activo el movimiento
        _canDrag = true;
        _originParent = transform.parent;
        _originIndex = transform.GetSiblingIndex();
        transform.SetParent(_originParent.parent);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    { // lo muevo donde tenga el cursor
        if (!_canDrag) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_canDrag) return;
        //le doy los datos de donde ha caido
        _canDrag = false;
        _canvasGroup.blocksRaycasts = true;
        transform.SetParent(_originParent);
        if (!_dropOnItem)
        { // si no sobre otro item, volver a origen
            transform.SetSiblingIndex(_originIndex);
        }
        transform.localPosition = Vector3.zero;
        _dropOnItem = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!IsWagonMenu()) return;
        Invent_Items dragged = eventData.pointerDrag?.GetComponent<Invent_Items>();
        if (dragged == null) return;
        {// si caemos sobre otro item
            dragged._dropOnItem = true;
            // reordenamos los index de los LayoutGroups
            int myIndex = transform.GetSiblingIndex();
            dragged.transform.SetParent(_originParent);
            dragged.transform.SetSiblingIndex(myIndex);
            transform.SetSiblingIndex(dragged._originIndex);
        }
    }
}

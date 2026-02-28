using UnityEngine;

public class Buy_Buttons : MonoBehaviour
{//script en cada boton de compra especifico
    public Inventory_Manager _IM; //pillo SINGLE del IM
    public Item_Data item; // asigno ScripItem

    private void Start()
    { if (_IM == null) { _IM = Inventory_Manager._IM; } }
    public void OnBuyItem()
    {
        if (_IM != null && item != null)
        { _IM.BuyItem(item); }
    }
}

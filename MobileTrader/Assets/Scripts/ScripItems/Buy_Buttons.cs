using UnityEngine;

public class Buy_Buttons : MonoBehaviour
{//script en cada boton de compra especifico
    public Invent_Control _IC; //pillo SINGLE del IM
    public Item_Data item; // asigno ScripItem

    private void Start()
    { if (_IC == null) { _IC = Invent_Control._IC; } }
    public void OnBuyItem()
    {
        if (_IC != null && item != null)
        { _IC.BuyItem(item); }
    }
}

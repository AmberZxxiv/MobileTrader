using UnityEngine;

public class Buy_Buttons : MonoBehaviour
{//script en cada boton de compra especifico
    public Invent_Control _IM; //pillo SINGLE del IM
    public Item_Data item; // asigno ScripItem

    private void Start()
    { if (_IM == null) { _IM = Invent_Control._IC; } }
    public void OnBuyItem()
    {
        if (_IM != null && item != null)
        { _IM.BuyItem(item); }
    }
}

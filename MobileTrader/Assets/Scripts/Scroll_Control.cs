using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using ClassicTouch = UnityEngine.Touch;
using UnityEngine.InputSystem.LowLevel;

public class Scroll_Control : MonoBehaviour
{// script en el canvas porque tiene que estar en algun lao
    public static Scroll_Control _SC; //SINGLE
    CurrentMenu currentMenu;
    enum CurrentMenu
    {
        Tent,
        Wagon,
        Merchant
    }
    #region /// WAGON MENU ///
    public GameObject tentMenu;
    public GameObject wagonMenu;
    public GameObject merchMenu;
    public GameObject inventoryGrid;
    public Vector3 wagonInvent;
    public Vector3 merchInvent;
    #endregion

    #region /// PARCELS LIST ///
    public List<GameObject> parceList;
    public List<Parcel_Limiter> cropsParcels;
    public List<Parcel_Limiter> farmParcels;
    public int currentParcel;
    #endregion

    #region /// CONTROL DESPLAZAMIENTO ///
    public float scrollForce;
    Vector2 _touchStart;
    float _topScreen;
    float _bottomScreen;
    #endregion

    void Awake() // SINGLE cosas
    {
        if (_SC == null) _SC = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // cojo los espacios relativos de la pantalla
        _topScreen = Screen.height * 0.75f;
        _bottomScreen = Screen.height * 0.25f;

        // aparezco con el Tent plegado
        ShowTent();

        // aseguro que solo activo la parcela 0 al iniciar
        for (int i = 0; i < parceList.Count; i++)
        { parceList[i].SetActive(i == currentParcel);}

        // conteo las parcelas del tipo Crops y Farm
        cropsParcels.Clear();
        foreach (GameObject parcelUnit in parceList)
        {
            Parcel_Limiter parcel = parcelUnit.GetComponent<Parcel_Limiter>();
            if (parcel != null && parcel.parcelType == Parcel_Limiter.ParcelType.Crops)
            { cropsParcels.Add(parcel); }
        }
        farmParcels.Clear();
        foreach (GameObject parcelUnit in parceList)
        {
            Parcel_Limiter parcel = parcelUnit.GetComponent<Parcel_Limiter>();
            if (parcel != null && parcel.parcelType == Parcel_Limiter.ParcelType.Farm)
            { farmParcels.Add(parcel); }
        }
    }

    void Update()
    {
        if (Input.touchCount > 0) //pal tactil del movil
        {
            // calculo desde donde a donde toco y genero un movimiento en la direccion
            ClassicTouch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            { _touchStart = touch.position; }
            if (touch.phase == TouchPhase.Ended)
            { SwipeDirector(touch.position);  }
        }

        #if UNITY_EDITOR // pal PC hago lo mismo con el raton
        if (Input.GetMouseButtonDown(0)) 
        { _touchStart = Input.mousePosition; }
        if (Input.GetMouseButtonUp(0))
        { SwipeDirector(Input.mousePosition); }
        #endif
    }

    void SwipeDirector(Vector2 endPos)
    {
        // pillo el movimiento, y si tiene la fuerza suficiente lo comparo
        Vector2 delta = endPos - _touchStart;
        if (delta.magnitude < scrollForce) return;

        bool isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
        bool isVertical = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
        float bottomZoneHeight = _bottomScreen;
        bool startedInTopZone = _touchStart.y > _topScreen;
        bool startedInBottomZone = _touchStart.y < bottomZoneHeight;

        if (isHorizontal) //movimiento horizontal para parcelas
        {
            if (delta.x < 0) NextParcel();
            if (delta.x > 0) PreviousParcel();
        }
        // Desde Tent abajo a arriba, activo Wagon
        if (isVertical && startedInBottomZone && delta.y > 0 && currentMenu == CurrentMenu.Tent)
        {
            ShowWagon();
            return;
        }
        // Desde Wagon arriba a abajo, vuelvo a Tent
        if (isVertical && startedInTopZone && delta.y < 0 && currentMenu == CurrentMenu.Wagon)
        {
            ShowTent();
            return;
        }
        // Desde Wagon abajo a arriba, voy a Merchant
        if (isVertical && startedInBottomZone && delta.y > 0 && currentMenu == CurrentMenu.Wagon)
        {
            ShowMerchant();
            return;
        }
        // Desde Merchant arrib a abajo, vuelvo a Wagon
        if (isVertical && startedInTopZone && delta.y < 0 && currentMenu == CurrentMenu.Merchant)
        {
            ShowWagon();
            return;
        }
    }

    public void NextParcel()
    { // sumo 1 en la lista de parcelas y si no hay mas, doy la vuelta
        if (parceList == null || parceList.Count == 0) return;
        parceList[currentParcel].SetActive(false);
        currentParcel = (currentParcel + 1) % parceList.Count;
        parceList[currentParcel].SetActive(true);
    }
    public void PreviousParcel()
    {// resto 1 en la lista de parcelas y si no hay mas, doy la vuelta
        if (parceList == null || parceList.Count == 0) return;
        parceList[currentParcel].SetActive(false);
        currentParcel = (currentParcel - 1 + parceList.Count) % parceList.Count;
        parceList[currentParcel].SetActive(true);
    }
    void ShowTent() // actualizo los menus a tent
    {
        inventoryGrid.SetActive(false);
        tentMenu.SetActive(true);
        wagonMenu.SetActive(false);
        merchMenu.SetActive(false);
        currentMenu = CurrentMenu.Tent;
    }
    void ShowWagon() // actualizo los menus a wagon
    {
        inventoryGrid.SetActive(true);
        RectTransform rt = inventoryGrid.GetComponent<RectTransform>();
        rt.anchoredPosition = wagonInvent;
        tentMenu.SetActive(false);
        wagonMenu.SetActive(true);
        merchMenu.SetActive(false);
        currentMenu = CurrentMenu.Wagon;
    }
    void ShowMerchant() // actualizo los menus a merchant
    {
        inventoryGrid.SetActive(true);
        RectTransform rt = inventoryGrid.GetComponent<RectTransform>();
        rt.anchoredPosition = merchInvent;
        tentMenu.SetActive(false);
        wagonMenu.SetActive(false);
        merchMenu.SetActive(true);
        currentMenu = CurrentMenu.Merchant;
    }
}

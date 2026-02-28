using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using ClassicTouch = UnityEngine.Touch;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting.Antlr3.Runtime;
using TMPro;

public class Scroll_Control : MonoBehaviour
{// script en el canvas porque tiene que estar en algun lao
    public static Scroll_Control _SC; //SINGLE
    public enum CurrentMenu
    { Tent, Wagon, Merchant }
    public CurrentMenu currentMenu;

    #region /// WAGON MENU ///
    public GameObject tentMenu;
    public GameObject wagonMenu;
    public List<GameObject> merchMenus;
    int _currentMerch;
    public GameObject inventoryGrid;
    public Vector3 wagonInvent;
    public Vector3 merchInvent;
    public int moneyCount;
    public TextMeshProUGUI moneyNumber;
    #endregion

    #region /// PARCELS LIST ///
    public List<GameObject> parceList;
    public List<Parcel_Limiter> baseParcels;
    public List<Parcel_Limiter> cropsParcels;
    public List<Parcel_Limiter> farmParcels;
    public int currentParcel;
    public GameObject basePref;
    public GameObject cropsPref;
    public GameObject farmPref;
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

        // conteo las parcelas de cada tipo
        baseParcels.Clear();
        foreach (GameObject parcelUnit in parceList)
        {
            Parcel_Limiter parcel = parcelUnit.GetComponent<Parcel_Limiter>();
            if (parcel != null && parcel.parcelType == Parcel_Limiter.ParcelType.Base)
            {
                baseParcels.Add(parcel);
            }
        }
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

        if (isHorizontal) //movimiento horizontal dependiendo del menú
        {
            switch (currentMenu)
            {
                case CurrentMenu.Merchant:
                    if (delta.x < 0) NextMerchMenu();       // swipe izquierda
                    if (delta.x > 0) PreviousMerchMenu();  // swipe derecha
                    break;

                case CurrentMenu.Wagon:
                case CurrentMenu.Tent:
                    if (delta.x < 0) NextParcel();      // swipe izquierda
                    if (delta.x > 0) PreviousParcel(); // swipe derecha
                    break;
            }
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
        parceList[currentParcel].SetActive(true);
        inventoryGrid.SetActive(false);
        tentMenu.SetActive(true);
        wagonMenu.SetActive(false);
        DisableMerchMenus();
        currentMenu = CurrentMenu.Tent;
    }
    void ShowWagon() // actualizo los menus a wagon
    {
        parceList[currentParcel].SetActive(false);
        inventoryGrid.SetActive(true);
        RectTransform rt = inventoryGrid.GetComponent<RectTransform>();
        rt.anchoredPosition = wagonInvent;
        tentMenu.SetActive(false);
        wagonMenu.SetActive(true);
        DisableMerchMenus();
        currentMenu = CurrentMenu.Wagon;
    }
    void ShowMerchant() // actualizo los menus a merchant
    {
        parceList[currentParcel].SetActive(false);
        inventoryGrid.SetActive(true);
        RectTransform rt = inventoryGrid.GetComponent<RectTransform>();
        rt.anchoredPosition = merchInvent;

        tentMenu.SetActive(false);
        wagonMenu.SetActive(false);

        _currentMerch = 0;
        for (int i = 0; i < merchMenus.Count; i++)
        { merchMenus[i].SetActive(i == _currentMerch); }

        currentMenu = CurrentMenu.Merchant;
    }
    void NextMerchMenu()
    {
        if (merchMenus.Count == 0) return;
        merchMenus[_currentMerch].SetActive(false);
        _currentMerch = (_currentMerch + 1) % merchMenus.Count;
        merchMenus[_currentMerch].SetActive(true);
    }
    void PreviousMerchMenu()
    {
        if (merchMenus.Count == 0) return;
        merchMenus[_currentMerch].SetActive(false);
        _currentMerch = (_currentMerch - 1 + merchMenus.Count) % merchMenus.Count;
        merchMenus[_currentMerch].SetActive(true);
    }
    void DisableMerchMenus()
    {
        foreach (GameObject menu in merchMenus)
        { menu.SetActive(false);}
    }
    public void UpdateMoney(int amount)
    {
        moneyCount += amount;
        moneyNumber.text = "x"+moneyCount.ToString();
    }
    public void ToAddBase()
    {
        GameObject newParcel = Instantiate(basePref);
        Parcel_Limiter pl = newParcel.GetComponent<Parcel_Limiter>();
        RegisterParcel(pl);
        newParcel.SetActive(false);
    }
    public void ToAddCrops()
    {
        GameObject newParcel = Instantiate(cropsPref);
        Parcel_Limiter pl = newParcel.GetComponent<Parcel_Limiter>();
        RegisterParcel(pl);
        newParcel.SetActive(false);
    }
    public void ToAddFarm()
    {
        GameObject newParcel = Instantiate(farmPref);
        Parcel_Limiter pl = newParcel.GetComponent<Parcel_Limiter>();
        RegisterParcel(pl);
        newParcel.SetActive(false);
    }
    public void RegisterParcel(Parcel_Limiter parcelNEW)
    {
        // lista general
        if (!parceList.Contains(parcelNEW.gameObject))
            parceList.Add(parcelNEW.gameObject);

        // específicas
        switch (parcelNEW.parcelType)
        {
            case Parcel_Limiter.ParcelType.Base:
                if (!baseParcels.Contains(parcelNEW))
                    baseParcels.Add(parcelNEW);
                break;

            case Parcel_Limiter.ParcelType.Crops:
                if (!cropsParcels.Contains(parcelNEW))
                    cropsParcels.Add(parcelNEW);
                break;

            case Parcel_Limiter.ParcelType.Farm:
                if (!farmParcels.Contains(parcelNEW))
                    farmParcels.Add(parcelNEW);
                break;
        }
    }
}

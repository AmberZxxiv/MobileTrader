using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using ClassicTouch = UnityEngine.Touch;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting.Antlr3.Runtime;
using TMPro;
using UnityEngine.SceneManagement;

public class Zones_General : MonoBehaviour
{// script en el canvas porque tiene que estar en algun lao
    public static Zones_General _ZG; // declaro SINGLE
    public enum CurrentMenu
    { Tent, Wagon, Merchant }
    public CurrentMenu currentMenu;

    #region /// WAGON MENU ///
    public GameObject tentMenu;
    public GameObject wagonMenu;
    public List<GameObject> merchMenus;
    int _currentMerch;
    public GameObject paginasGrid;
    public Vector3 wagonInvent;
    public Vector3 merchInvent;
    public int moneyCount;
    public TextMeshProUGUI moneyNumber;
    #endregion

    #region /// PARCELS LIST ///
    public List<GameObject> parceList;
    public List<Parcel_Individual> baseParcels;
    public List<Parcel_Individual> cropsParcels;
    public List<Parcel_Individual> farmParcels;
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

    void Awake() // declaro SINGLE
    { if (_ZG == null) _ZG = this; else Destroy(gameObject); }
    void Start()
    {
        Time.timeScale = 1;
        // cojo espacios relativos de pantalla
        _topScreen = Screen.height * 0.75f;
        _bottomScreen = Screen.height * 0.25f;
        // conteo parcelas de cada tipo
        InitializeParcels();
        // aseguro que activo parcela 0 al iniciar
        for (int i = 0; i < parceList.Count; i++)
        { parceList[i].SetActive(i == currentParcel);}
    }
    void Update()
    {
        //Movimiento unificado para tactil y raton
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = Vector2.zero;
        bool released = false;

        if (Input.touchCount > 0)
        {
            ClassicTouch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            { _touchStart = touch.position; }
            if (touch.phase == TouchPhase.Ended)
            { endPos = touch.position; released = true;}
        }
        else if (Input.GetMouseButtonDown(0))
        {_touchStart = Input.mousePosition;}
        else if (Input.GetMouseButtonUp(0))
        {endPos = Input.mousePosition;released = true;}
        if (released)
        { SwipeDirector(endPos); }
    }

    void InitializeParcels()
    {
        baseParcels.Clear();
        cropsParcels.Clear();
        farmParcels.Clear();
        foreach (GameObject parcelUnit in parceList)
        {
            Parcel_Individual parcel = parcelUnit.GetComponent<Parcel_Individual>();
            if (parcel == null) continue;
            switch (parcel.parcelType)
            {
                case Parcel_Individual.ParcelType.Base:
                    baseParcels.Add(parcel); break;

                case Parcel_Individual.ParcelType.Crops:
                    cropsParcels.Add(parcel); break;

                case Parcel_Individual.ParcelType.Farm:
                    farmParcels.Add(parcel); break;
            }
        }
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
                case CurrentMenu.Wagon:
                    if (delta.x < 0) Invent_Control._IC.NextPage();       // swipe izquierda
                    if (delta.x > 0) Invent_Control._IC.PreviousPage();  // swipe derecha
                    break;
                case CurrentMenu.Tent:
                    if (delta.x < 0) NextParcel();      // swipe izquierda
                    if (delta.x > 0) PreviousParcel(); // swipe derecha
                    break;
                case CurrentMenu.Merchant:
                    if (delta.x < 0) NextMerchMenu();       // swipe izquierda
                    if (delta.x > 0) PreviousMerchMenu();  // swipe derecha
                    break;
            }
        }
        if (isVertical && startedInTopZone && delta.y < 0 && currentMenu == CurrentMenu.Wagon)
        {// Desde Wagon arriba a abajo, vuelvo a Tent
            ShowTent();
            return;
        }
        if (isVertical && startedInBottomZone && delta.y > 0 && currentMenu == CurrentMenu.Tent)
        {// Desde Tent abajo a arriba, activo Wagon
            ShowWagon();
            return;
        }
        if (isVertical && startedInBottomZone && delta.y > 0 && currentMenu == CurrentMenu.Wagon)
        {// Desde Wagon abajo a arriba, voy a Merchant
            ShowMerchant();
            return;
        }
        if (isVertical && startedInTopZone && delta.y < 0 && currentMenu == CurrentMenu.Merchant)
        {// Desde Merchant arriba a abajo, vuelvo a Wagon
            ShowWagon();
            return;
        }
    }
    public void NextParcel()// sumo 1 en lista general parcelas
    { ChangeParcel(1);}
    public void PreviousParcel()// resto 1 en la lista de parcelas
    { ChangeParcel(-1);}
    void ChangeParcel(int direction)
    {
        if (parceList == null || parceList.Count == 0) return;
        parceList[currentParcel].SetActive(false);
        currentParcel = (currentParcel + direction + parceList.Count) % parceList.Count;
        parceList[currentParcel].SetActive(true);
    }
    void ChangeMenu(CurrentMenu newMenu)
    {
        currentMenu = newMenu;
        bool showParcel = (newMenu == CurrentMenu.Tent);
        parceList[currentParcel].SetActive(showParcel);

        paginasGrid.SetActive(newMenu != CurrentMenu.Tent);
        tentMenu.SetActive(newMenu == CurrentMenu.Tent);
        wagonMenu.SetActive(newMenu == CurrentMenu.Wagon);
        DisableMerchMenus();

        if (newMenu == CurrentMenu.Wagon)
        {
            paginasGrid.GetComponent<RectTransform>().anchoredPosition = wagonInvent;
            Invent_Control._IC.ShowPage(Invent_Control._IC.currentPage);
        }
        if (newMenu == CurrentMenu.Merchant)
        {
            paginasGrid.GetComponent<RectTransform>().anchoredPosition = merchInvent;
            _currentMerch = 0;
            UpdateMerchMenu();
        }
    }
    void ShowTent() // actualizo los menus a tent
    { ChangeMenu(CurrentMenu.Tent); }
    void ShowWagon() // actualizo los menus a wagon
    { ChangeMenu(CurrentMenu.Wagon);}
    void ShowMerchant() // actualizo los menus a merchant
    { ChangeMenu(CurrentMenu.Merchant);}
    void ChangeMerchMenu(int direction)
    {
        if (merchMenus.Count == 0) return;
        merchMenus[_currentMerch].SetActive(false);
        _currentMerch = (_currentMerch + direction + merchMenus.Count) % merchMenus.Count;
        merchMenus[_currentMerch].SetActive(true);
    }
    void NextMerchMenu() // sumo 1 en lista de mercados
    { ChangeMerchMenu(1); }
    void PreviousMerchMenu() // resto 1 en lista de mercados
    { ChangeMerchMenu(-1); }
    void UpdateMerchMenu() // actualizo al mercado actual
    {
        for (int i = 0; i < merchMenus.Count; i++)
        { merchMenus[i].SetActive(i == _currentMerch); }
    }
    void DisableMerchMenus() // desactivo todos los mercados
    { foreach (GameObject menu in merchMenus) menu.SetActive(false); }

    void AddParcel(GameObject prefab) // llaman tras comprar parcelas en Inventory
    {
        GameObject newParcel = Instantiate(prefab);
        Parcel_Individual pl = newParcel.GetComponent<Parcel_Individual>();
        RegisterParcel(pl);
        newParcel.SetActive(false);
    }
    public void ToAddBase()
    { AddParcel(basePref); }
    public void ToAddCrops()
    { AddParcel(cropsPref); }
    public void ToAddFarm()
    { AddParcel(farmPref); }
    public void RegisterParcel(Parcel_Individual parcelNEW) // Start de cada Parcel Limiter
    {
        // todas a la general
        if (!parceList.Contains(parcelNEW.gameObject))
            parceList.Add(parcelNEW.gameObject);
        // específicas
        switch (parcelNEW.parcelType)
        {
            case Parcel_Individual.ParcelType.Base:
                if (!baseParcels.Contains(parcelNEW))
                baseParcels.Add(parcelNEW); break;
            case Parcel_Individual.ParcelType.Crops:
                if (!cropsParcels.Contains(parcelNEW))
                cropsParcels.Add(parcelNEW); break;
            case Parcel_Individual.ParcelType.Farm:
                if (!farmParcels.Contains(parcelNEW))
                farmParcels.Add(parcelNEW); break;
        }
    }
    public void UpdateMoney(int amount) // lo llamo al comprar o vender
    {
        moneyCount += amount;
        moneyNumber.text = "x" + moneyCount.ToString();
    }

    public void ExitGame()
    { SceneManager.LoadScene(0);}
}

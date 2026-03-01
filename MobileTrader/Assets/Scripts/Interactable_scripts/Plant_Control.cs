using UnityEngine;
using UnityEngine.EventSystems;

public class Plant_Control : MonoBehaviour
{// script en cada prefab de planta

    public enum  PlantState
    { Planted, Grown }
    public PlantState plantState;

    #region /// PRODUCTION ///
    public Item_Data producedItem;
    public float grownTime;
    float _timerCounter;
    #endregion

    #region /// STATE ///
    public GameObject plantedChild;
    public GameObject grownChild;
    #endregion

    void Start()
    {
        // pillo los hijos propios
        plantedChild = transform.GetChild(0).gameObject;
        grownChild = transform.GetChild(1).gameObject;
    }
    void Update()
    {
        // de base, sumo el timer hasta llegar al grown
        if (plantState == PlantState.Planted)
        {
            _timerCounter += Time.deltaTime;
            // cuando llego al grown, cambio el estado
            if (_timerCounter >= grownTime)
            { SetState(PlantState.Grown);}
        }
    }
    void SetState(PlantState newState) // actualizo de uno a otro
    {
        plantState = newState;
        switch (plantState)
        {
            case PlantState.Planted:
                plantedChild.SetActive(true);
                grownChild.SetActive(false);
                _timerCounter = 0f; break;
            case PlantState.Grown:
                plantedChild.SetActive(false);
                grownChild.SetActive(true); break;
        }
    }
    void OnMouseDown()
    {
        // si esta crecido, puedo recoger al tapear
        if (plantState == PlantState.Grown)
        { Harvest(); }
    }
    void Harvest()
    {
        // clicar sobre la planta añade su producto al inventario
        if (producedItem != null)
        { Invent_Control._IC.AddItem(producedItem); }
        // reseteo el estado base
        SetState(PlantState.Planted);
    }
}

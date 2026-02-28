using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel_Limiter : MonoBehaviour
{// script padre prefab de cada parcela
    public static Parcel_Limiter _PL; //SINGLE PARCELA ACTUAL 

    public enum ParcelType
    { Base, Crops, Farm }
    public ParcelType parcelType;
    public enum ResourceType
    { Pet, Plant, Animal }
    public ResourceType resourceType;

    #region /// SPACE LIMITS ///
    public Collider2D parcelBounds;
    public Transform gridStart;
    public int gridColumns;
    public int gridRows;
    public float gridSpacing;
    public int animalLimit;
    #endregion

    // relaciono los objetos en listas
    Dictionary<ResourceType, List<GameObject>> resources = new Dictionary<ResourceType, List<GameObject>>()
    {
    { ResourceType.Plant, new List<GameObject>() },
    { ResourceType.Animal, new List<GameObject>() },
    { ResourceType.Pet, new List<GameObject>() }
    };

    private void Start() // llamo al registro del S_Control
    { Scroll_Control._SC.RegisterParcel(this); }

    void OnEnable() // cuando activo la nueva parcela
    {
        _PL = this; // SINGLE PARCELA ACTUAL 
        // cojo controladores de todos los hijos animales y les activo el limitador
        if (parcelType == ParcelType.Farm || parcelType == ParcelType.Base)
        {
            Animal_Control[] animals = GetComponentsInChildren<Animal_Control>();
            foreach (Animal_Control animal in animals)
            { animal.SetParcelBounds(parcelBounds); }
        }
    }
    public bool ToAddResource(GameObject prefab, ResourceType type) // desde Inventario al Comprar
    {
        if (!HasSpace(type)) { print(type + " lleno"); return false; }
        var list = resources[type];
        UnityEngine.Vector3 spawnPos = gridStart.position;
        if (type == ResourceType.Plant) //plantas desde esquina grid
        {
            int index = list.Count;
            int row = index / gridColumns;
            int col = index % gridColumns;
            spawnPos.x += col * gridSpacing;
            spawnPos.y -= row * gridSpacing;
        }
        else // animales desde circulo central
        { spawnPos += new UnityEngine.Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0); }
        // instancio el recurso correspondiente
        GameObject newEntity = Instantiate(prefab, spawnPos, UnityEngine.Quaternion.identity, transform);
        list.Add(newEntity);
        return true;
    }
    public bool HasSpace(ResourceType type)
    { // cojo los límites de cada recurso
        var list = resources[type];
        if (type == ResourceType.Plant)
        return (list.Count / gridColumns) < gridRows;
        else
        return list.Count < animalLimit;
    }
    // desde Inventario al Comprar
    public bool HasSpaceForPlant()
    { return HasSpace(ResourceType.Plant); }
    public bool HasSpaceForAnimal()
    { return HasSpace(ResourceType.Animal); }
    public bool HasSpaceForPets()
    { return HasSpace(ResourceType.Pet); }
}

using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel_Limiter : MonoBehaviour
{// script padre prefab de cada parcela
    public static Parcel_Limiter _PL; //SINGLE
    
    public enum ParcelType
    { Base, Crops, Farm }
    public ParcelType parcelType;

    public Collider2D parcelBounds;
    public Transform gridStart;
    public int gridColumns;
    public int gridRows;
    public float gridSpacing;
    public int animalLimit;
    List<GameObject> _plantsCount = new List<GameObject>();
    List<GameObject> _animalCount = new List<GameObject>();
    List<GameObject> _petsCount = new List<GameObject>();


    private void Start()
    {
        // llamo al registro de listas
        Scroll_Control._SC.RegisterParcel(this);
    }

    void OnEnable() // cuando activo la nueva parcela
    {
        _PL = this; // SINGLE ACTUAL 

        // cojo controladores de todos los hijos animales y les activo el limitador
        if (parcelType == ParcelType.Farm || parcelType == ParcelType.Base)
        {
            Animal_Control[] animals = GetComponentsInChildren<Animal_Control>();
            foreach (Animal_Control animal in animals)
            { animal.SetParcelBounds(parcelBounds); }
        }
    }
    public bool ToAddPlant(GameObject plantPrefabCustom)
    {
        if (!HasSpaceForPlant())
        { print("Crops llenas"); return false; }

        int index = _plantsCount.Count;
        int row = index / gridColumns;
        int col = index % gridColumns;

        UnityEngine.Vector3 pos = gridStart.position;
        pos.x += col * gridSpacing;
        pos.y -= row * gridSpacing;

        GameObject newPlant = Instantiate(plantPrefabCustom, pos, UnityEngine.Quaternion.identity, transform);
        _plantsCount.Add(newPlant);
        return true;
    }

    public bool ToAddAnimal(GameObject animalPrefabCustom)
    {
        if (!HasSpaceForAnimal())
        { print("Farms llenas"); return false; }

        UnityEngine.Vector3 offset = new UnityEngine.Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        UnityEngine.Vector3 pos = gridStart.position + offset;

        GameObject newAnimal = Instantiate(animalPrefabCustom, pos, UnityEngine.Quaternion.identity, transform);
        _animalCount.Add(newAnimal);
        return true;
    }
    public bool ToAddPet(GameObject petPrefab)
    {
        if (!HasSpaceForPets())
        {print("Mascotas llenas"); return false;}

        UnityEngine.Vector3 offset = new UnityEngine.Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        UnityEngine.Vector3 pos = gridStart.position + offset;

        GameObject newPet = Instantiate(petPrefab, pos, UnityEngine.Quaternion.identity, transform);
        _petsCount.Add(newPet);
        return true;
    }
    public bool HasSpaceForPlant()
    {
        int index = _plantsCount.Count;
        int row = index / gridColumns;
        return row < gridRows;
    }
    public bool HasSpaceForAnimal()
    {
        return _animalCount.Count < animalLimit;
    }
    public bool HasSpaceForPets()
    {
        return _petsCount.Count < animalLimit;
    }
}

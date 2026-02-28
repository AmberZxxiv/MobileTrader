using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel_Limiter : MonoBehaviour
{// script padre prefab de cada parcela
    public static Parcel_Limiter _PL; //SINGLE
    public ParcelType parcelType;
    public enum ParcelType
    {
        Base,
        Crops,
        Farm
    }
    public Collider2D parcelBounds;
    public Transform gridStart;
    public int gridColumns;
    public int gridRows;
    public float gridSpacing;

    public GameObject plantPrefab;
    List<GameObject> _plantsCount = new List<GameObject>();

    public GameObject animalPrefab;
    List<GameObject> _animalCount = new List<GameObject>();
    public int animalLimit;

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
    public bool ToAddAnimal()
    {
        if (_animalCount.Count >= animalLimit)
        {print("Farm llena");return false;}
        // spawn con ligero límite evita superposicion
        UnityEngine.Vector3 offset = new UnityEngine.Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        UnityEngine.Vector3 pos = gridStart.position + offset;
        GameObject newAnimal = Instantiate(animalPrefab, pos, UnityEngine.Quaternion.identity);
        _animalCount.Add(newAnimal);
        // instanciamos animal
        Animal_Control ac = newAnimal.GetComponent<Animal_Control>();
        return true;
    }
    public bool ToAddPlant()
    {
        int index = _plantsCount.Count;
        int row = index / gridColumns;
        int col = index % gridColumns;
        if (row >= gridRows)
        { print("Crop llena"); return false;}

        // gridStart como esquina superior izquierda
        UnityEngine.Vector3 pos = gridStart.position;
        pos.x += col * gridSpacing;   // izquierda a derecha
        pos.y -= row * gridSpacing;   // arriba a abajo
        // instancio siguiente planta
        GameObject newPlant = Instantiate(plantPrefab, pos, UnityEngine.Quaternion.identity);
        _plantsCount.Add(newPlant);
        return true;
    }
}

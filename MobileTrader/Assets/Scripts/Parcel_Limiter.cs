using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel_Limiter : MonoBehaviour
{// script en empty padre de prefab parcelas completas
    // declaro universalmente la parcela
    public static Parcel_Limiter activeParcel; 

    // declaro prefab correspondiente del bioma con collider
    public Collider2D parcelBounds;
    public GameObject plantPrefab;
    public Transform gridStart;
    public int gridColumns;
    public float gridSpacing;
    private List<GameObject> gridCount = new List<GameObject>();

    void OnEnable() // cuando activo la nueva parcela
    {
        activeParcel = this; // declaro esta parcela activa

        // cojo los controladores de todos los hijos animales y les activo el limitador
        Animal_Control[] animals = GetComponentsInChildren<Animal_Control>();
        foreach (Animal_Control animal in animals)
        { animal.SetParcelBounds(parcelBounds); }
    }

    void Update()
    {
        // pulsando P añado planta
        if (Input.GetKeyDown(KeyCode.P) && activeParcel != null)
        { activeParcel.AddPlant(); }
    }

    public void AddPlant()
    {
        int index = gridCount.Count;
        int row = index / gridColumns;
        int col = index % gridColumns;

        // posición inicial de las columnas
        UnityEngine.Vector3 pos = gridStart.position 
        + new UnityEngine.Vector3(col * gridSpacing, 0, row * gridSpacing);
        // limite dentro del collider de la parcela
        pos.x = Mathf.Clamp(pos.x, parcelBounds.bounds.min.x, parcelBounds.bounds.max.x);
        pos.z = Mathf.Clamp(pos.z, parcelBounds.bounds.min.y, parcelBounds.bounds.max.y);

        GameObject newPlant = Instantiate(plantPrefab, pos, UnityEngine.Quaternion.identity);
        gridCount.Add(newPlant);
    }
}

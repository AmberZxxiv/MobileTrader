using UnityEngine;

public class Parcel_Limiter : MonoBehaviour
{
    // aqui le doy el backround fisico de la parcela con el collider
    public Collider2D parcelBounds;

    void OnEnable() // cuando activo la nueva parcela
    {
        // cojo los controladores de todos los hijos animales y les activo el limitador
        Animal_Control[] animals = GetComponentsInChildren<Animal_Control>();
        foreach (Animal_Control animal in animals)
        { animal.SetParcelBounds(parcelBounds); }
    }
}

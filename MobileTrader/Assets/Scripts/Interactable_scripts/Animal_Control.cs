using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using static Plant_Control;

public class Animal_Control : MonoBehaviour
{// script en cada prefab de animal
    public enum AnimalState
    { None, Ready }
    public AnimalState animalState;

    #region /// MOVIMIENTO ///
    public float speed;
    Vector2 _direction;
    Bounds _bounds;
    #endregion

    #region /// PRODUCTION ///
    public Item_Data producedItem;
    public float cooldownTime;
    float _timerCounter;
    public GameObject particleReady;
    #endregion

    void Start()
    {
        // le doy dirección random
        _direction = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        // si tengo limites asignados, lo muevo hasta que choque
        if (_bounds.size == Vector3.zero) return;
        transform.Translate(_direction * speed * Time.deltaTime);
        CheckBounds();

        // de base, sumo el timer hasta llegar a ready
        if (animalState == AnimalState.None)
        {
            _timerCounter += Time.deltaTime;
            // cuando llego a ready, doy feedback del estado
            if (_timerCounter >= cooldownTime)
            { SetState(AnimalState.Ready); }
        }
    }

    void OnMouseDown() // tap sobre animal ready activa recoleccion
    {
        if (animalState != AnimalState.Ready) return;
        Harvest();
    }
    void Harvest()
    {
        if (producedItem != null)
        {
            Invent_Control._IC.AddItem(producedItem);
            if (particleReady != null)
            {
                Vector3 spawnPos = transform.position;
                Quaternion spawnRot = Quaternion.Euler(90f, 0f, 0f);
                Instantiate(particleReady, spawnPos, spawnRot);
            }
        }
        SetState(AnimalState.None);
    }
    void SetState(AnimalState newState)
    {
        animalState = newState;
        _timerCounter = 0f;
    }

    public void SetParcelBounds(Collider2D parcel) // pillo Parcel_Individual
    { _bounds = parcel.bounds; }
    void CheckBounds()
    {
        // cojo la posicion del objeto y al llegar al límite la cambio
        Vector3 pos = transform.position;
        float limits = 0.1f;
        if (pos.x < _bounds.min.x + limits || pos.x > _bounds.max.x - limits)
        { _direction.x *= -1;}
        if (pos.y < _bounds.min.y + limits || pos.y > _bounds.max.y - limits)
        {_direction.y *= -1; }
    }
}

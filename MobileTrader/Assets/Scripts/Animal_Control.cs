using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class Animal_Control : MonoBehaviour
{// script en cada prefab de animal
    #region /// MOVIMIENTO ///
    public float speed;
    Vector2 _direction;
    Bounds _bounds;
    #endregion

    #region /// PRODUCTION ///
    public Item_Data producedItem;
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
    }

    void OnMouseDown() // clicar sobre el animal para recoger su producto
    { Inventory_Manager.Instance.AddItem(producedItem); }

    public void SetParcelBounds(Collider2D parcel) // pillo Parcel_Limiter
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

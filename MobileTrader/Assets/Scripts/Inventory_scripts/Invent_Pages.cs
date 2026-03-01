using System.Collections.Generic;
using UnityEngine;

public class Invent_Pages : MonoBehaviour
{ // script en prefab padre de las paginas que van en el canvas
   
    int _maxSlots = 12;
    public Transform gridInvent;// grid hijo
    private Dictionary<Item_Data, Invent_Items> slots = 
        new Dictionary<Item_Data, Invent_Items>();

    public bool HasSpace()
    { return slots.Count < _maxSlots; }

    public bool TryAddItem(Item_Data item, Invent_Items prefab)
    {
        // Buscar si ya existe el item en la página
        foreach (var exist in slots)
        {
            if (exist.Key == item)
            {
                exist.Value.AddAmount(item.collectAmount); return true;
            }
        }

        // Si hay espacio para un nuevo slot
        if (slots.Count >= _maxSlots) return false;

        // Instanciar slot
        Invent_Items newSlot = Instantiate(prefab, gridInvent);
        newSlot.Setup(item, item.collectAmount);
        slots.Add(item, newSlot); return true;
    }
    public bool TryRemoveItem(Item_Data item)
    {
        if (!slots.ContainsKey(item))
            return false;

        slots[item].DecreaseAmount(1);

        if (slots[item].Amount <= 0)
        {
            Destroy(slots[item].gameObject);
            slots.Remove(item);
        }

        return true;
    }
}

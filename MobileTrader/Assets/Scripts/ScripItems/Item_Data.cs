using UnityEngine;

// ScriptableObject para crear en assets diferentes items
[CreateAssetMenu(menuName = "Farm/Item")]
public class Item_Data : ScriptableObject
{ //esto se rellena en cada uno
    public string itemName;
    public Sprite icon;
    public int collectAmount = 1;
    public int sellPrice = 1;
    public int buyAmount = 1;
    public int buyPrice = 1;
    public enum ItemType { Deco, Pet, Plant, Animal }
    public ItemType type;
    public GameObject prefab;
}

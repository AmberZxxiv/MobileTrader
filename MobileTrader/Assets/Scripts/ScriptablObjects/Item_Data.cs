using UnityEngine;

// ScriptableObject para crear en assets diferentes items
[CreateAssetMenu(menuName = "Farm/Item")]
public class Item_Data : ScriptableObject
{ //esto re rellena en cada uno
    public string itemName;
    public Sprite icon;
    public int amount;
}

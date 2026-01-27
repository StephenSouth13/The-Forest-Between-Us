using UnityEngine;

// Đảm bảo phần dưới này giống hệt như sau
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int maxStackSize = 99;
}
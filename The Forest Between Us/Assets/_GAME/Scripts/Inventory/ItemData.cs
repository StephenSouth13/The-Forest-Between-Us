//D:\VTC_Academy\game3d\The-Forest-Between-Us\The Forest Between Us\Assets\Scripts\Inventory\ItemData.cs
using UnityEngine;

// Định nghĩa các loại vật phẩm để làm bộ lọc
public enum ItemType { All, Weapon, Food, Resource }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType category; // Nhãn để bộ lọc nhận diện
    public Sprite icon;
    public bool isStackable;
    public int maxStackSize = 99;
}
//D:\VTC_Academy\game3d\The-Forest-Between-Us\The Forest Between Us\Assets\Scripts\Inventory\Interactable.cs
using UnityEngine;

public interface Interactable
{
    string GetInteractPrompt(); // Trả về chuỗi hiển thị khi người chơi nhìn vào vật thể
    void OnInteract(); // Hàm này sẽ được gọi khi người chơi nhấn phím
}
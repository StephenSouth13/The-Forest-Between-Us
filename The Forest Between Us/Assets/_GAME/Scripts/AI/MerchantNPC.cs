using UnityEngine;

public class MerchantNPC : MonoBehaviour, Interactable
{
    public string merchantName = "Thương Nhân Lữ Hành";

    [TextArea(2, 4)]
    public string greetingText = "Chào mừng bạn, tôi có một vài món hàng quý giá muốn trao đổi lấy Vỏ Sò. Bạn có hứng thú không?";

    public string GetInteractPrompt()
    {
        return $"Giao thương với {merchantName} (Phím E)";
    }

    public void OnInteract()
    {
        if (DialogueChoiceUIController.instance != null)
        {
            DialogueChoiceUIController.instance.ShowChoices(
                merchantName, greetingText,
                "Xem hàng hóa", () => {
                    if (TradingUIController.instance != null)
                    {
                        TradingUIController.instance.OpenStore();
                    }
                    else
                    {
                        Debug.LogWarning("Không tìm thấy TradingUIController!");
                    }
                },
                "Rời đi", () => {
                    Debug.Log("Kết thúc giao thương.");
                }
            );
        }
        else
        {
            // Fallback nếu không có UI
            Debug.Log($"[{merchantName}]: {greetingText}");
            if (TradingUIController.instance != null)
            {
                TradingUIController.instance.OpenStore();
            }
        }
    }
}

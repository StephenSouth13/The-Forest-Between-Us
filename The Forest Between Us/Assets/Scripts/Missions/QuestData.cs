using UnityEngine;

// Dòng này giúp bạn chuột phải trong Project -> Create -> Quest System -> Quest để tạo nhiệm vụ mới
[CreateAssetMenu(fileName = "Day_01_Quest", menuName = "Quest System/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Thời điểm xuất hiện")]
    public int dayRequired; // Nhiệm vụ này dành cho ngày thứ mấy? (1 đến 30)

    [Header("Thông tin nhiệm vụ")]
    public string questName;        // Tên nhiệm vụ (vd: Tìm nguồn nước)
    [TextArea(3, 10)]
    public string description;      // Mô tả chi tiết

    [Header("Điều kiện hoàn thành")]
    public ItemData targetItem;     // Vật phẩm cần nhặt (nếu có)
    public int requiredAmount;      // Số lượng cần (vd: nhặt 5 cành củi)
    public int currentAmount;       // Số lượng hiện tại đang có

    [HideInInspector]
    public bool isCompleted;        // Đánh dấu đã xong chưa
}
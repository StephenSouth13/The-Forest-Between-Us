using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryLoreDatabase", menuName = "Forest Between Us/Story Lore Database")]
public class StoryLoreDatabase : ScriptableObject
{
    [Header("Story Lore Logs")]
    [TextArea(5, 12)]
    public string prologueLore = "Ngày 0: Tín hiệu vô tuyến đầu tiên đứt đoạn trong sương mù. Giọng nói vượt thời gian của Mai An Tiêm...";
    
    [TextArea(5, 12)]
    public string day1Lore = "Ngày 1: Hạt Đen phát quang và sự biến dạng của ranh giới thực tại. Hãy cẩn trọng những âm thanh trong sương.";

    [TextArea(5, 12)]
    public string day2Lore = "Ngày 2: Sinh thể bóng đêm xuất hiện. Thu thập linh kiện để chế tạo Đuốc Tần Số và Bộ Lọc Đài.";

    [TextArea(5, 12)]
    public string day3Lore = "Ngày 3: Trạm Phát Sóng Trọng Tâm. Lẻn qua kẻ thù, khôi phục mạch điện và đưa ra quyết định vận mệnh.";

    [Header("Quests List")]
    public List<QuestData> mainQuests = new List<QuestData>();
}

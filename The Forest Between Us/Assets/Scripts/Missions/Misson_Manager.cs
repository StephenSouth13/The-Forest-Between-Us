using UnityEngine;
using System.Collections.Generic;

public class Misson_Manager : MonoBehaviour
{
    public int currentDay = 1;
    
    [Header("Quest Database")]
    // Kéo tất cả các file QuestData (Day_01, Day_02...) vào đây trong Inspector
    public List<QuestData> allQuests = new List<QuestData>();

    void Start()
    {
        UpdateDailyQuest();
    }

    public void UpdateDailyQuest()
    {
        // Kiểm tra xem ngày hiện tại có nằm trong danh sách không
        if (currentDay <= allQuests.Count)
        {
            // Lấy dữ liệu Quest của ngày tương ứng (Index bắt đầu từ 0 nên lấy currentDay - 1)
            QuestData questToday = allQuests[currentDay - 1];
            
            // Đưa cả GÓI dữ liệu này cho QuestManager xử lý
            QuestManager.instance.InitializeQuest(questToday);
        }
        else
        {
            Debug.Log("No more quests available for Day " + currentDay);
        }
    }
}
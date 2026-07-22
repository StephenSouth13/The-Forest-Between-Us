using UnityEngine;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public int currentDay = 1;
    
    [Header("Quest Database")]
    public List<QuestData> allQuests = new List<QuestData>();

    [Header("Tutorial Objects")]
    public GameObject radioObject; // Kéo cái Radio 3D vào đây

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Lúc đầu vào game, ẩn cái đài đi cho đúng kịch bản
        if (radioObject != null) radioObject.SetActive(false);

        UpdateDailyQuest();
    }

    public void UpdateDailyQuest()
    {
        if (QuestManager.instance == null)
        {
            Debug.LogWarning("QuestManager is missing. Cannot initialize daily quest.");
            return;
        }

        if (currentDay > 0 && currentDay <= allQuests.Count)
        {
            QuestData questToday = allQuests[currentDay - 1];
            QuestManager.instance.InitializeQuest(questToday);
        }
        else
        {
            Debug.Log("No more quests available for Day " + currentDay);
        }
    }

    // Hàm này sẽ được TutorialManager gọi khi bấm xong 8 nút
    public void ActivateRadio()
    {
        if (radioObject != null)
        {
            radioObject.SetActive(true);
            Debug.Log("Radio Activated! Go find it.");
        }
    }
}

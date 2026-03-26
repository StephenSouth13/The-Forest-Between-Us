using UnityEngine;
using System.Collections.Generic; // Phải có dòng này mới dùng được List
using TMPro; // Nếu bạn dùng TextMeshPro để hiện chữ nhiệm vụ

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Data")]
    public List<QuestData> allQuests; // Danh sách 30 file QuestData của bạn
    public QuestData activeQuest;

    [Header("UI")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescText;

    private void Awake()
    {
        instance = this;
    }

    public void ActivateQuestForDay(int day)
    {
        // Tìm nhiệm vụ có dayRequired khớp với ngày hiện tại
        activeQuest = allQuests.Find(q => q.dayRequired == day);

        if (activeQuest != null)
        {
            ShowQuestOnUI(activeQuest);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy nhiệm vụ cho ngày: " + day);
        }
    }

    void ShowQuestOnUI(QuestData quest)
    {
        if (questTitleText != null) questTitleText.text = quest.questName;
        if (questDescText != null) questDescText.text = quest.description;
        
        Debug.Log("Nhiệm vụ mới: " + quest.questName);
    }
}
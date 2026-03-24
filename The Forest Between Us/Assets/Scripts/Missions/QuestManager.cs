using UnityEngine;
public class QuestManager : MonoBehaviour
{
    public List<QuestManager> allQuests;
    public QuestData activeQuest;
    public void ActivateQuestForDay(int day)
    {
        activeQuest = allQuests.Find(q => q.requiredDay == day);
        if (activeQuest != null)
        {
            ShowQuestOnUI(activeQuest);
        }
    }
}

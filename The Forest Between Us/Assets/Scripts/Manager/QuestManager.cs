using UnityEngine;
using TMPro;
using System.Collections;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Current Progress")]
    public QuestData activeQuest;
    private int currentStepIndex = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI storyOverlay; // Kéo Text dẫn truyện vào đây

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (activeQuest != null) InitializeQuest(activeQuest);
    }

    public void InitializeQuest(QuestData newQuest)
    {
        activeQuest = newQuest;
        currentStepIndex = 0;
        
        // Reset progress in Data
        foreach (var step in activeQuest.steps) {
            step.currentAmount = 0;
            step.isFinished = false;
        }

        // --- MỚI: Xử lý Story Overlay (Dẫn truyện) ---
        if (storyOverlay != null && !string.IsNullOrEmpty(newQuest.storyIntro)) {
            storyOverlay.text = newQuest.storyIntro;
            storyOverlay.gameObject.SetActive(true);
            // Tắt chữ dẫn truyện sau 5 giây để bắt đầu hiện nhiệm vụ
            Invoke("HideStoryOverlay", 5f);
        }

        UpdateUI();
        Debug.Log("Quest Started: " + activeQuest.questTitle);
    }

    void HideStoryOverlay() {
        if (storyOverlay != null) storyOverlay.gameObject.SetActive(false);
    }

    public void UpdateObjectiveText(string customText)
    {
        if (objectiveText != null) objectiveText.text = customText;
    }

    public void AdvanceStep(StepType type, float amount = 1f)
    {
        if (activeQuest == null || currentStepIndex >= activeQuest.steps.Count) return;

        QuestStep currentStep = activeQuest.steps[currentStepIndex];

        if (currentStep.type == type && !currentStep.isFinished)
        {
            currentStep.currentAmount += amount;

            if (currentStep.currentAmount >= currentStep.targetAmount)
            {
                currentStep.isFinished = true;
                currentStepIndex++; 
                
                if (currentStepIndex >= activeQuest.steps.Count)
                {
                    CompleteQuest();
                }
            }
            UpdateUI();
        }
    }

    public void UpdateUI() // Đổi thành public để TutorialManager có thể gọi
    {
        if (activeQuest == null || titleText == null || objectiveText == null) return;

        titleText.text = activeQuest.questTitle.ToUpper();

        if (currentStepIndex < activeQuest.steps.Count)
        {
            var s = activeQuest.steps[currentStepIndex];
            objectiveText.text = $"- {s.description} ({Mathf.RoundToInt(s.currentAmount)}/{s.targetAmount})";
        }
        else
        {
            objectiveText.text = "Day Objectives Completed.";
        }
    }

    void CompleteQuest()
    {
        activeQuest.isCompleted = true;
        Debug.Log("Quest Finished!");
    }
}
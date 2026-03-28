using UnityEngine;
using TMPro; // Use TextMeshPro for high-quality text

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Current Progress")]
    public QuestData activeQuest;
    private int currentStepIndex = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI storyOverlay; // For the cinematic intro

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
        
        // Reset all steps to 0 for a fresh start
        foreach (var step in activeQuest.steps) {
            step.currentAmount = 0;
            step.isFinished = false;
        }

        UpdateUI();
        Debug.Log("Quest Started: " + activeQuest.questTitle);
    }

    // This is the CORE function. Call this from other scripts!
    // Example: QuestManager.instance.AdvanceStep(StepType.Collect, 1);
    public void AdvanceStep(StepType type, int amount = 1)
    {
        if (activeQuest == null || currentStepIndex >= activeQuest.steps.Count) return;

        QuestStep currentStep = activeQuest.steps[currentStepIndex];

        // Check if the action matches the current requirement
        if (currentStep.type == type && !currentStep.isFinished)
        {
            currentStep.currentAmount += amount;

            if (currentStep.currentAmount >= currentStep.targetAmount)
            {
                currentStep.isFinished = true;
                currentStepIndex++; // Move to the NEXT step in the list
                
                if (currentStepIndex >= activeQuest.steps.Count)
                {
                    CompleteQuest();
                }
            }
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        titleText.text = activeQuest.questTitle.ToUpper();

        if (currentStepIndex < activeQuest.steps.Count)
        {
            var s = activeQuest.steps[currentStepIndex];
            // Shows: "Find the Radio (0/1)"
            objectiveText.text = $"- {s.description} ({s.currentAmount}/{s.targetAmount})";
        }
        else
        {
            objectiveText.text = "Day Objectives Completed.";
        }
    }

    void CompleteQuest()
    {
        activeQuest.isCompleted = true;
        Debug.Log("Day " + activeQuest.dayID + " successfully finished!");
        // Logic to transition to the next day or show a summary screen
    }
}
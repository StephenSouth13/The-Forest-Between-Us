using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Current Progress")]
    public QuestData activeQuest;
    private int currentStepIndex = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI storyOverlay; 

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

        UpdateUI();
        Debug.Log("Quest Started: " + activeQuest.questTitle);
    }

    // --- NEW FUNCTION: Needed for Tutorial distance tracking ---
    public void UpdateObjectiveText(string customText)
    {
        objectiveText.text = customText;
    }

    // --- UPDATED: Changed 'int amount' to 'float amount' ---
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

    void UpdateUI()
    {
        if (activeQuest == null) return;

        titleText.text = activeQuest.questTitle.ToUpper();

        if (currentStepIndex < activeQuest.steps.Count)
        {
            var s = activeQuest.steps[currentStepIndex];
            // Uses Mathf.RoundToInt so "99.8/100m" looks like "100/100m"
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
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("Current Progress")]
    public QuestData activeQuest;
    private int currentStepIndex;

    [Header("UI Reference")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI storyOverlay;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (activeQuest != null) InitializeQuest(activeQuest);
    }

    public void InitializeQuest(QuestData newQuest)
    {
        activeQuest = newQuest;
        currentStepIndex = 0;

        if (activeQuest == null) return;

        foreach (QuestStep step in activeQuest.steps)
        {
            step.currentAmount = 0;
            step.isFinished = false;
        }

        if (storyOverlay != null && !string.IsNullOrEmpty(newQuest.storyIntro))
        {
            storyOverlay.text = newQuest.storyIntro;
            storyOverlay.gameObject.SetActive(true);
            Invoke(nameof(HideStoryOverlay), 5f);
        }

        UpdateUI();
        Debug.Log("Quest started: " + activeQuest.questTitle);
    }

    void HideStoryOverlay()
    {
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

        if (currentStep.type != type || currentStep.isFinished) return;

        currentStep.currentAmount += amount;

        if (currentStep.currentAmount >= currentStep.targetAmount)
        {
            currentStep.isFinished = true;
            currentStepIndex++;

            if (currentStepIndex >= activeQuest.steps.Count)
            {
                CompleteQuest();
                return;
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (activeQuest == null || titleText == null || objectiveText == null) return;

        titleText.text = activeQuest.questTitle.ToUpper();

        if (currentStepIndex < activeQuest.steps.Count)
        {
            QuestStep step = activeQuest.steps[currentStepIndex];
            objectiveText.text = $"- {step.description} ({Mathf.RoundToInt(step.currentAmount)}/{step.targetAmount})";
        }
        else
        {
            objectiveText.text = "Day objectives completed.";
        }
    }

    void CompleteQuest()
    {
        activeQuest.isCompleted = true;
        UpdateUI();
        Debug.Log("Quest finished: " + activeQuest.questTitle);
    }
}

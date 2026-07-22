using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Controls Panel")]
    public GameObject keysPanel;
    public Color activeColor = Color.green;
    public bool hidePromptWhenPressed = true;
    public float promptFadeDuration = 0.2f;

    [Header("Legacy Key Images")]
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX, imgTab, imgB, imgF;

    [Header("Custom Key Prompts")]
    public List<TutorialKeyPrompt> keyPrompts = new List<TutorialKeyPrompt>();

    [Header("Goal Tracking")]
    public Transform playerTransform;
    public Transform goalTransform;
    public float finishDistance = 3f;
    public string reachRadioObjective = "Reach the radio signal.";

    private readonly List<TutorialKeyPrompt> activePrompts = new List<TutorialKeyPrompt>();
    private bool controlsDone;
    private bool tutorialComplete;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        BuildPromptList();
        if (keysPanel != null) keysPanel.SetActive(true);
    }

    void Update()
    {
        if (tutorialComplete) return;

        if (!controlsDone)
        {
            CheckKeyInputs();
            UpdatePromptFades();
        }
        else
        {
            TrackDistanceToGoal();
        }
    }

    void BuildPromptList()
    {
        activePrompts.Clear();

        if (keyPrompts.Count > 0)
        {
            activePrompts.AddRange(keyPrompts);
        }
        else
        {
            AddLegacyPrompt(KeyCode.W, imgW);
            AddLegacyPrompt(KeyCode.A, imgA);
            AddLegacyPrompt(KeyCode.S, imgS);
            AddLegacyPrompt(KeyCode.D, imgD);
            AddLegacyPrompt(KeyCode.Space, imgSpace);
            AddLegacyPrompt(KeyCode.LeftShift, imgShift);
            AddLegacyPrompt(KeyCode.C, imgC);
            AddLegacyPrompt(KeyCode.X, imgX);
            AddLegacyPrompt(KeyCode.Tab, imgTab);
            AddLegacyPrompt(KeyCode.B, imgB);
            AddLegacyPrompt(KeyCode.F, imgF);
        }

        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            prompt.Initialize();
        }
    }

    void AddLegacyPrompt(KeyCode keyCode, Image image)
    {
        activePrompts.Add(new TutorialKeyPrompt
        {
            keyCode = keyCode,
            promptObject = image != null ? image.gameObject : null,
            promptGraphic = image,
            completedColor = activeColor
        });
    }

    void CheckKeyInputs()
    {
        bool allComplete = activePrompts.Count > 0;

        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            if (!prompt.IsComplete && Input.GetKeyDown(prompt.keyCode))
            {
                prompt.MarkComplete(hidePromptWhenPressed, promptFadeDuration);
            }

            if (!prompt.IsComplete) allComplete = false;
        }

        if (allComplete)
        {
            CompleteControlsTutorial();
        }
    }

    void UpdatePromptFades()
    {
        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            prompt.UpdateFade(Time.unscaledDeltaTime);
        }
    }

    void CompleteControlsTutorial()
    {
        controlsDone = true;

        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            prompt.HideImmediate();
        }

        if (keysPanel != null) keysPanel.SetActive(false);

        QuestManager.instance?.AdvanceStep(StepType.Movement, 1);
        QuestManager.instance?.UpdateObjectiveText(reachRadioObjective);
        MissionManager.instance?.ActivateRadio();

        Debug.Log("Tutorial controls complete. Reach the radio signal.");
    }

    void TrackDistanceToGoal()
    {
        if (playerTransform == null || goalTransform == null) return;

        float distanceToGoal = Vector3.Distance(playerTransform.position, goalTransform.position);

        if (distanceToGoal > finishDistance)
        {
            QuestManager.instance?.UpdateObjectiveText(
                $"{reachRadioObjective} {Mathf.RoundToInt(distanceToGoal)}m");
        }
        else
        {
            FinishTutorial();
        }
    }

    void FinishTutorial()
    {
        tutorialComplete = true;
        QuestManager.instance?.AdvanceStep(StepType.ReachTarget, 1);
        Debug.Log("Arrived at the first radio signal. Tutorial complete.");
    }
}

[System.Serializable]
public class TutorialKeyPrompt
{
    public KeyCode keyCode;
    public GameObject promptObject;
    public Graphic promptGraphic;
    public CanvasGroup canvasGroup;
    public Color completedColor = Color.green;

    public bool IsComplete { get; private set; }

    private bool fading;
    private float fadeTimer;
    private float fadeDuration = 0.2f;

    public void Initialize()
    {
        IsComplete = false;
        fading = false;
        fadeTimer = 0f;

        if (promptObject != null) promptObject.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    public void MarkComplete(bool hideWhenPressed, float duration)
    {
        IsComplete = true;

        if (promptGraphic != null) promptGraphic.color = completedColor;

        if (!hideWhenPressed)
        {
            return;
        }

        fadeDuration = Mathf.Max(0.01f, duration);
        fading = true;
        fadeTimer = 0f;

        if (canvasGroup == null)
        {
            HideImmediate();
        }
    }

    public void UpdateFade(float deltaTime)
    {
        if (!fading || canvasGroup == null) return;

        fadeTimer += deltaTime;
        canvasGroup.alpha = Mathf.Clamp01(1f - fadeTimer / fadeDuration);

        if (fadeTimer >= fadeDuration)
        {
            HideImmediate();
        }
    }

    public void HideImmediate()
    {
        fading = false;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (promptObject != null) promptObject.SetActive(false);
    }
}

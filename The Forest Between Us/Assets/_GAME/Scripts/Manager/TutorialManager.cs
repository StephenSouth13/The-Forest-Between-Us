using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Controls Panel")]
    public GameObject keysPanel;
    public Color activeColor = new Color(0.2f, 1f, 0.6f, 1f); // Sleek Neon Cyan-Green
    public bool hidePromptWhenPressed = true;
    public float promptFadeDuration = 0.35f;
    public bool enableBreathingPulse = true;
    public bool playAudioFeedback = true;

    [Header("Legacy Key Images")]
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX, imgTab, imgB, imgF;

    [Header("Custom Key Prompts")]
    public List<TutorialKeyPrompt> keyPrompts = new List<TutorialKeyPrompt>();

    [Header("Goal Tracking & 3D Waypoint")]
    public Transform playerTransform;
    public Transform goalTransform;
    public float finishDistance = 3f;
    public string reachRadioObjective = "Reach the radio signal.";
    public GameObject waypointMarkerPrefab;

    private readonly List<TutorialKeyPrompt> activePrompts = new List<TutorialKeyPrompt>();
    private bool controlsDone;
    private bool tutorialComplete;
    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void Start()
    {
        BuildPromptList();
        if (keysPanel != null) keysPanel.SetActive(true);

        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        if (goalTransform == null)
        {
            GameObject r = GameObject.Find("SM_Radio");
            if (r != null) goalTransform = r.transform;
        }
    }

    void Update()
    {
        if (tutorialComplete) return;

        if (!controlsDone)
        {
            CheckKeyInputs();
            UpdatePromptAnimations();
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
            prompt.Initialize(activeColor);
        }
    }

    void AddLegacyPrompt(KeyCode keyCode, Image image)
    {
        if (image == null) return;
        activePrompts.Add(new TutorialKeyPrompt
        {
            keyCode = keyCode,
            promptObject = image.gameObject,
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
                if (playAudioFeedback) PlayKeyClickSFX();
            }

            if (!prompt.IsComplete) allComplete = false;
        }

        if (allComplete)
        {
            CompleteControlsTutorial();
        }
    }

    void UpdatePromptAnimations()
    {
        float time = Time.unscaledTime;
        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            prompt.UpdateAnimation(Time.unscaledDeltaTime, time, enableBreathingPulse);
        }
    }

    void CompleteControlsTutorial()
    {
        controlsDone = true;

        if (playAudioFeedback) PlayCompletionChimeSFX();

        foreach (TutorialKeyPrompt prompt in activePrompts)
        {
            prompt.HideImmediate();
        }

        if (keysPanel != null) keysPanel.SetActive(false);

        QuestManager.instance?.AdvanceStep(StepType.Movement, 1);
        QuestManager.instance?.UpdateObjectiveText($"🎯 {reachRadioObjective}");
        MissionManager.instance?.ActivateRadio();

        Debug.Log("Tutorial controls complete. Reach the radio signal.");
    }

    void TrackDistanceToGoal()
    {
        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        if (playerTransform == null || goalTransform == null) return;

        float distanceToGoal = Vector3.Distance(playerTransform.position, goalTransform.position);

        if (distanceToGoal > finishDistance)
        {
            QuestManager.instance?.UpdateObjectiveText(
                $"🎯 {reachRadioObjective} ({Mathf.RoundToInt(distanceToGoal)}m)");
        }
        else
        {
            FinishTutorial();
        }
    }

    void FinishTutorial()
    {
        tutorialComplete = true;
        if (playAudioFeedback) PlayCompletionChimeSFX();
        QuestManager.instance?.AdvanceStep(StepType.ReachTarget, 1);
        Debug.Log("Arrived at the first radio signal. Tutorial complete.");
    }

    // Sound Synthesizers for Instant AAA Audio Feedback without needing external assets
    void PlayKeyClickSFX()
    {
        if (audioSource == null) return;
        AudioClip clip = CreateToneClip(880f, 0.08f, 0.25f);
        audioSource.PlayOneShot(clip);
    }

    void PlayCompletionChimeSFX()
    {
        if (audioSource == null) return;
        AudioClip clip = CreateChordClip(new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.4f, 0.3f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreateToneClip(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(1f - t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }
        AudioClip clip = AudioClip.Create("KeySFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip CreateChordClip(float[] frequencies, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(1f - t / duration);
            float mix = 0f;
            foreach (float f in frequencies)
            {
                mix += Mathf.Sin(2f * Mathf.PI * f * t);
            }
            data[i] = (mix / frequencies.Length) * envelope * volume;
        }
        AudioClip clip = AudioClip.Create("ChimeSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}

[System.Serializable]
public class TutorialKeyPrompt
{
    public KeyCode keyCode;
    public GameObject promptObject;
    public Graphic promptGraphic;
    public CanvasGroup canvasGroup;
    public Color completedColor = new Color(0.2f, 1f, 0.6f, 1f);

    public bool IsComplete { get; private set; }

    private bool fading;
    private float fadeTimer;
    private float fadeDuration = 0.35f;

    private Vector3 originalScale = Vector3.one;
    private RectTransform rectTransform;
    private Color originalColor = Color.white;
    private float punchScaleFactor = 1f;

    public void Initialize(Color defaultActiveColor)
    {
        IsComplete = false;
        fading = false;
        fadeTimer = 0f;
        punchScaleFactor = 1f;

        if (promptObject != null)
        {
            promptObject.SetActive(true);
            rectTransform = promptObject.GetComponent<RectTransform>();
            if (rectTransform != null) originalScale = rectTransform.localScale;
        }

        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (promptGraphic != null) originalColor = promptGraphic.color;
        if (completedColor == Color.green) completedColor = defaultActiveColor;
    }

    public void MarkComplete(bool hideWhenPressed, float duration)
    {
        IsComplete = true;

        if (promptGraphic != null) promptGraphic.color = completedColor;
        punchScaleFactor = 1.35f; // Instant Punch Bounce Effect

        if (!hideWhenPressed) return;

        fadeDuration = Mathf.Max(0.01f, duration);
        fading = true;
        fadeTimer = 0f;

        if (canvasGroup == null)
        {
            HideImmediate();
        }
    }

    public void UpdateAnimation(float deltaTime, float globalTime, bool breathingPulse)
    {
        if (promptObject == null || !promptObject.activeSelf) return;

        // Smooth Punch Scale decay
        if (punchScaleFactor > 1f)
        {
            punchScaleFactor = Mathf.Lerp(punchScaleFactor, 1f, deltaTime * 12f);
        }

        // Apply scale & breathing pulse effect
        if (rectTransform != null)
        {
            float pulse = (breathingPulse && !IsComplete) ? Mathf.Sin(globalTime * 4f) * 0.05f : 0f;
            rectTransform.localScale = originalScale * (punchScaleFactor + pulse);
        }

        // Fade out transition when completed
        if (fading && canvasGroup != null)
        {
            fadeTimer += deltaTime;
            float progress = Mathf.Clamp01(fadeTimer / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            if (fadeTimer >= fadeDuration)
            {
                HideImmediate();
            }
        }
    }

    public void HideImmediate()
    {
        fading = false;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (promptObject != null) promptObject.SetActive(false);
    }
}


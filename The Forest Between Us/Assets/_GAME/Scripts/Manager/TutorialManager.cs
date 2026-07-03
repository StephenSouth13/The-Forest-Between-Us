using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI Keys Setup")]
    public GameObject keysPanel;
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX;
    public Color activeColor = Color.green;

    [Header("Goal Tracking")]
    public Transform playerTransform;
    public Transform goalTransform;
    public float finishDistance = 3f;

    private bool w, a, s, d, space, shift, c, x;
    private bool keysDone;
    private bool tutorialComplete;

    void Start()
    {
        instance = this;
        if (keysPanel != null) keysPanel.SetActive(true);
    }

    void Update()
    {
        if (tutorialComplete) return;

        if (!keysDone)
        {
            CheckKeyInputs();
        }
        else
        {
            TrackDistanceToGoal();
        }
    }

    void CheckKeyInputs()
    {
        if (Input.GetKeyDown(KeyCode.W)) { w = true; SetKeyActive(imgW); }
        if (Input.GetKeyDown(KeyCode.A)) { a = true; SetKeyActive(imgA); }
        if (Input.GetKeyDown(KeyCode.S)) { s = true; SetKeyActive(imgS); }
        if (Input.GetKeyDown(KeyCode.D)) { d = true; SetKeyActive(imgD); }
        if (Input.GetKeyDown(KeyCode.Space)) { space = true; SetKeyActive(imgSpace); }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { shift = true; SetKeyActive(imgShift); }
        if (Input.GetKeyDown(KeyCode.C)) { c = true; SetKeyActive(imgC); }
        if (Input.GetKeyDown(KeyCode.X)) { x = true; SetKeyActive(imgX); }

        if (w && a && s && d && space && shift && c && x)
        {
            keysDone = true;
            Debug.Log("Keys mastered. Radio signal is now active.");
            QuestManager.instance?.AdvanceStep(StepType.Movement, 1);
            Misson_Manager.instance?.ActivateRadio();
        }
    }

    void SetKeyActive(Image image)
    {
        if (image != null) image.color = activeColor;
    }

    void TrackDistanceToGoal()
    {
        if (playerTransform == null || goalTransform == null) return;

        float distanceToGoal = Vector3.Distance(playerTransform.position, goalTransform.position);

        if (distanceToGoal > finishDistance)
        {
            QuestManager.instance?.UpdateObjectiveText(
                $"Reach the Radio Station: {Mathf.RoundToInt(distanceToGoal)}m away");
        }
        else
        {
            FinishTutorial();
        }
    }

    void FinishTutorial()
    {
        tutorialComplete = true;
        if (keysPanel != null) keysPanel.SetActive(false);

        QuestManager.instance?.AdvanceStep(StepType.ReachTarget, 1);
        Debug.Log("Arrived at the first radio signal. Tutorial complete.");
    }
}

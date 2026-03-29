using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI Keys Setup")]
    public GameObject keysPanel; 
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX;
    public Color activeColor = Color.green; 

    [Header("Goal Tracking (Mốc mục tiêu)")]
    public Transform playerTransform; 
    public Transform goalTransform;     // Kéo Object đích đến (ví dụ: Đài Radio) vào đây
    public float finishDistance = 3f;   // Khoảng cách đủ gần để tính là tới nơi

    [Header("Status Flags")]
    private bool w, a, s, d, space, shift, c, x;
    private bool keysDone = false;
    private bool tutorialComplete = false;

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
        if (Input.GetKeyDown(KeyCode.W)) { w = true; imgW.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.A)) { a = true; imgA.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.S)) { s = true; imgS.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.D)) { d = true; imgD.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.Space)) { space = true; imgSpace.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { shift = true; imgShift.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.C)) { c = true; imgC.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.X)) { x = true; imgX.color = activeColor; }

        if (w && a && s && d && space && shift && c && x)
        {
            keysDone = true;
            Debug.Log("Keys Mastered! Now reach the target.");
            // Báo QuestManager xong bước bấm nút
            QuestManager.instance.AdvanceStep(StepType.Movement, 1);
        }
    }

    void TrackDistanceToGoal()
    {
        if (playerTransform == null || goalTransform == null) return;

        // Tính khoảng cách hiện tại giữa người chơi và đích
        float distanceToGoal = Vector3.Distance(playerTransform.position, goalTransform.position);

        if (distanceToGoal > finishDistance)
        {
            // Hiện lên màn hình: "Reach the Radio Station: 85m"
            string distText = $"Reach the Radio Station: {Mathf.RoundToInt(distanceToGoal)}m away";
            QuestManager.instance.UpdateObjectiveText(distText);
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
        
        // Báo cho QuestManager xong bước di chuyển đến đích
        QuestManager.instance.AdvanceStep(StepType.Movement, 1);
        Debug.Log("Arrived at destination! Tutorial Complete.");
    }
}
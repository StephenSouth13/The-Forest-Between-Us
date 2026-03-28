using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI Keys Panels")]
    public GameObject keysPanel; // Kéo cái Panel chứa ảnh W,A,S,D,Space... vào đây
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX;
    public Color activeColor = Color.green;

    [Header("Distance Settings")]
    public Transform playerTransform;
    private Vector3 lastPosition;
    private float totalDistanceMoved = 0f;
    public float requiredDistance = 100f;

    [Header("Status")]
    private bool w, a, s, d, space, shift, c, x;
    private bool keysDone = false;

    void Start() {
        instance = this;
        lastPosition = playerTransform.position;
        keysPanel.SetActive(true); // Hiện nút lên ngay khi vào game
    }

    void Update() {
        if (!keysDone) CheckKeys();
        else TrackDistance();
    }

    void CheckKeys() {
        if (Input.GetKeyDown(KeyCode.W)) { w = true; imgW.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.A)) { a = true; imgA.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.S)) { s = true; imgS.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.D)) { d = true; imgD.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.Space)) { space = true; imgSpace.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { shift = true; imgShift.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.C)) { c = true; imgC.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.X)) { x = true; imgX.color = activeColor; }

        if (w && a && s && d && space && shift && c && x) {
            keysDone = true;
            Debug.Log("Keys Mastered! Now run 100m.");
            // Cập nhật text nhiệm vụ sang: "Run 100m to stabilize signal"
            QuestManager.instance.AdvanceStep(StepType.Movement, 1); 
        }
    }

    void TrackDistance() {
        float distanceThisFrame = Vector3.Distance(playerTransform.position, lastPosition);
        totalDistanceMoved += distanceThisFrame;
        lastPosition = playerTransform.position;

        // Cập nhật UI nhiệm vụ (Ví dụ: "Travel: 45/100m")
        if (totalDistanceMoved < requiredDistance) {
            QuestManager.instance.UpdateObjectiveText($"Travel: {Mathf.RoundToInt(totalDistanceMoved)}/{requiredDistance}m");
        } else {
            FinishTutorial();
            this.enabled = false;
        }
    }

    void FinishTutorial() {
        keysPanel.SetActive(false); // Ẩn nút đi
        Debug.Log("Tutorial Complete!");
        QuestManager.instance.AdvanceStep(StepType.Movement, 1);
    }
}
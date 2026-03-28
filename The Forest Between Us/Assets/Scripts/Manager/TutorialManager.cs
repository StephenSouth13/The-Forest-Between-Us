using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("UI Keys Setup")]
    public GameObject keysPanel; // Cái Panel chứa 8 cái ảnh nút bấm
    // Kéo các Object Image từ Hierarchy vào đây
    public Image imgW, imgA, imgS, imgD, imgSpace, imgShift, imgC, imgX;
    public Color activeColor = Color.green; // Màu khi bấm xong

    [Header("Distance Tracking")]
    public Transform playerTransform; // Kéo PlayerArmature vào đây
    public float requiredDistance = 100f;
    
    private Vector3 lastPosition;
    private float totalDistanceMoved = 0f;

    [Header("Status Flags")]
    private bool w, a, s, d, space, shift, c, x;
    private bool keysDone = false;
    private bool tutorialComplete = false;

    void Start()
    {
        instance = this;
        if (playerTransform != null) lastPosition = playerTransform.position;
        keysPanel.SetActive(true); // Hiện bảng nút ngay khi vào game
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
            TrackPlayerDistance();
        }
    }

    void CheckKeyInputs()
    {
        // Kiểm tra từng phím một
        if (Input.GetKeyDown(KeyCode.W)) { w = true; imgW.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.A)) { a = true; imgA.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.S)) { s = true; imgS.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.D)) { d = true; imgD.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.Space)) { space = true; imgSpace.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.LeftShift)) { shift = true; imgShift.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.C)) { c = true; imgC.color = activeColor; }
        if (Input.GetKeyDown(KeyCode.X)) { x = true; imgX.color = activeColor; }

        // Khi bấm đủ 8 phím
        if (w && a && s && d && space && shift && c && x)
        {
            keysDone = true;
            Debug.Log("Keys Mastered! Start running 100m.");
            // Báo cho QuestManager xong Step 0 (Bấm nút)
            QuestManager.instance.AdvanceStep(StepType.Movement, 1);
        }
    }

    void TrackPlayerDistance()
    {
        if (playerTransform == null) return;

        // Tính quãng đường di chuyển giữa các frame
        float distanceThisFrame = Vector3.Distance(playerTransform.position, lastPosition);
        totalDistanceMoved += distanceThisFrame;
        lastPosition = playerTransform.position;

        // Cập nhật con số 0/100m lên màn hình thông qua QuestManager
        if (totalDistanceMoved < requiredDistance)
        {
            string distText = $"Travel: {Mathf.RoundToInt(totalDistanceMoved)}/{requiredDistance}m";
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
        keysPanel.SetActive(false); // Ẩn bộ nút đi cho sạch màn hình
        
        // Báo cho QuestManager xong Step 1 (Chạy 100m)
        QuestManager.instance.AdvanceStep(StepType.Movement, 1);
        Debug.Log("Tutorial Finished Successfully!");
    }
}
using UnityEngine;

public class UniversalSceneCameraController : MonoBehaviour
{
    public static UniversalSceneCameraController instance;

    [Header("🎥 MODE NÂNG CAO: ĐIỀU KHIỂN CAMERA CHO TOÀN BỘ SCENE")]
    public bool enableFreeFlyEditorMode = false;
    public float lookSensitivity = 2.0f;
    public float moveSpeed = 8.0f;
    public float fastMoveMultiplier = 2.5f;

    [Header("🎬 CINEMATIC & SCREEN SHAKE (HIỆU ỨNG RUNG LẮC CAMERA)")]
    public float defaultFov = 60f;
    public float sprintFov = 68f;

    private Camera cam;
    private float shakeTimer = 0f;
    private float shakeAmount = 0f;
    private Vector3 originalLocalPos;

    void Awake()
    {
        if (instance == null) instance = this;
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam != null) originalLocalPos = cam.transform.localPosition;
    }

    void Update()
    {
        HandleCameraShake();
        HandleFovDynamic();

        if (enableFreeFlyEditorMode)
        {
            HandleFreeFlyControl();
        }
    }

    // 💥 RUNG LẮC CAMERA KHI BỊ QUÁI ĐÁNH / CHẶT CÂY / NỔ
    public void TriggerCameraShake(float duration = 0.25f, float intensity = 0.15f)
    {
        shakeTimer = duration;
        shakeAmount = intensity;
    }

    void HandleCameraShake()
    {
        if (shakeTimer > 0)
        {
            cam.transform.localPosition = originalLocalPos + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            cam.transform.localPosition = originalLocalPos;
        }
    }

    // 🏃 THAY ĐỔI GÓC NHÌN FOV ĐỘNG KHI CHẠY SPRINT
    void HandleFovDynamic()
    {
        if (cam == null) return;
        bool isSprinting = PlayerStatsManager.instance != null && PlayerStatsManager.instance.isRunning;
        float targetFov = isSprinting ? sprintFov : defaultFov;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * 6f);
    }

    // 🛸 TÍNH NĂNG NÂNG CAO: BAY TỰ DO QUAN SÁT TOÀN BỘ SCENE (FREE FLY CAMERA)
    void HandleFreeFlyControl()
    {
        if (Input.GetMouseButton(1)) // Bấm giữ chuột phải để bay tự do
        {
            Cursor.lockState = CursorLockMode.Locked;
            float rotX = Input.GetAxis("Mouse X") * lookSensitivity;
            float rotY = Input.GetAxis("Mouse Y") * lookSensitivity;

            transform.Rotate(0, rotX, 0, Space.World);
            transform.Rotate(-rotY, 0, 0, Space.Self);

            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMoveMultiplier : 1.0f);
            Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.Translate(dir * speed * Time.deltaTime);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

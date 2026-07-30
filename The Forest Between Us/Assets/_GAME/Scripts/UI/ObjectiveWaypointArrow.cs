using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveWaypointArrow : MonoBehaviour
{
    public static ObjectiveWaypointArrow instance;

    [Header("Target & Camera")]
    public Transform targetTransform;
    public Transform playerTransform;
    public string targetObjectName = "SM_Radio";
    public string labelText = "RADIO SIGNAL";

    [Header("UI Element Customization")]
    public float screenMargin = 50f;
    public Color normalColor = new Color(0.2f, 1f, 0.6f, 1f); // Cyber Cyan
    public Color pulseColor = new Color(1f, 0.8f, 0.2f, 1f); // Gold Pulse

    private Camera mainCamera;
    private RectTransform canvasRect;
    private GameObject waypointGO;
    private RectTransform waypointRect;
    private Image arrowImage;
    private TextMeshProUGUI distanceText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();

        FindTargets();
        CreateWaypointUI();
    }

    void FindTargets()
    {
        if (targetTransform == null && !string.IsNullOrEmpty(targetObjectName))
        {
            GameObject targetGO = GameObject.Find(targetObjectName);
            if (targetGO != null) targetTransform = targetGO.transform;
        }

        if (playerTransform == null && mainCamera != null)
        {
            playerTransform = mainCamera.transform;
        }
    }

    void Update()
    {
        if (targetTransform == null || !targetTransform.gameObject.activeInHierarchy)
        {
            FindTargets();
            if (waypointGO != null) waypointGO.SetActive(targetTransform != null && targetTransform.gameObject.activeInHierarchy);
            if (targetTransform == null) return;
        }

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null || canvasRect == null) return;

        if (waypointGO != null && !waypointGO.activeSelf) waypointGO.SetActive(true);

        Vector3 targetWorldPos = targetTransform.position + Vector3.up * 1.5f; // Floating above radio
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        bool isBehind = screenPos.z < 0;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenBounds = new Vector2(Screen.width - screenMargin, Screen.height - screenMargin);

        if (isBehind)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        bool isOffScreen = isBehind || screenPos.x < screenMargin || screenPos.x > Screen.width - screenMargin || screenPos.y < screenMargin || screenPos.y > Screen.height - screenMargin;

        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(screenPos.x, screenMargin, Screen.width - screenMargin),
            Mathf.Clamp(screenPos.y, screenMargin, Screen.height - screenMargin)
        );

        // Position waypoint in Canvas space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, clampedPos, null, out localPoint);
        if (waypointRect != null) waypointRect.anchoredPosition = localPoint;

        // Rotate Arrow towards target if off screen
        if (arrowImage != null)
        {
            if (isOffScreen)
            {
                Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrowImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }
            else
            {
                arrowImage.rectTransform.localRotation = Quaternion.identity;
            }
        }

        // Calculate distance
        if (playerTransform != null && distanceText != null)
        {
            float dist = Vector3.Distance(playerTransform.position, targetTransform.position);
            distanceText.text = $"🔻 {labelText}\n<b>{Mathf.RoundToInt(dist)}m</b>";

            // Pulse color when close or searching
            float pulse = Mathf.Sin(Time.time * 6f) * 0.5f + 0.5f;
            Color currentColor = Color.Lerp(normalColor, pulseColor, pulse);
            if (arrowImage != null) arrowImage.color = currentColor;
        }
    }

    public void SetTarget(Transform newTarget, string newLabel = "RADIO SIGNAL")
    {
        targetTransform = newTarget;
        labelText = newLabel;
        if (waypointGO != null) waypointGO.SetActive(targetTransform != null);
    }

    void CreateWaypointUI()
    {
        if (canvasRect == null) return;

        waypointGO = new GameObject("Objective_Waypoint", typeof(RectTransform));
        waypointGO.transform.SetParent(canvasRect, false);

        waypointRect = waypointGO.GetComponent<RectTransform>();
        waypointRect.sizeDelta = new Vector2(100, 100);

        // Arrow Icon
        GameObject arrowGO = new GameObject("ArrowIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        arrowGO.transform.SetParent(waypointGO.transform, false);

        RectTransform arrowRect = arrowGO.GetComponent<RectTransform>();
        arrowRect.sizeDelta = new Vector2(36, 36);
        arrowRect.anchoredPosition = new Vector2(0, 15);

        arrowImage = arrowGO.GetComponent<Image>();
        arrowImage.color = normalColor;

        // Label & Distance Text
        GameObject textGO = new GameObject("DistanceText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(waypointGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(160, 50);
        textRect.anchoredPosition = new Vector2(0, -25);

        distanceText = textGO.GetComponent<TextMeshProUGUI>();
        distanceText.fontSize = 15f;
        distanceText.alignment = TextAlignmentOptions.Center;
        distanceText.color = Color.white;
        distanceText.text = $"🔻 {labelText}";
    }
}

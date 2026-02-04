using UnityEngine;
using UnityEngine.UI;

public class PlayerAvaterEffect : MonoBehaviour
{

    public static PlayerAvaterEffect instance;
    [Header("Refs")]
    public RectTransform minimapRect;        // RawImage minimap
    public RectTransform indicatorRect;      // PlayerIndicator (UI)
    public Transform player;                 // Player ngoài world
    public Camera minimapCamera;             // Camera minimap

    [Header("Effect")]
    public float pulseSpeed = 2f;
    public float pulseScale = 0.15f;

    private Vector3 baseScale;
    private bool isActive;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        baseScale = indicatorRect.localScale;
        indicatorRect.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        UpdateIndicatorPosition();
        PlayPulseEffect();
    }

    // Gọi khi mở minimap
    public void ShowIndicator()
    {
        isActive = true;
        indicatorRect.gameObject.SetActive(true);
    }

    // Gọi khi đóng minimap
    public void HideIndicator()
    {
        isActive = false;
        indicatorRect.gameObject.SetActive(false);
        indicatorRect.localScale = baseScale;
    }

    void UpdateIndicatorPosition()
    {
        // Chuyển world position -> viewport (0..1)
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(player.position);

        // Đổi viewport -> UI local position
        Vector2 minimapSize = minimapRect.sizeDelta;

        Vector2 uiPos = new Vector2(
            (viewportPos.x - 0.5f) * minimapSize.x,
            (viewportPos.y - 0.5f) * minimapSize.y
        );

        indicatorRect.anchoredPosition = uiPos;
    }

    void PlayPulseEffect()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        indicatorRect.localScale = baseScale * pulse;
    }

}

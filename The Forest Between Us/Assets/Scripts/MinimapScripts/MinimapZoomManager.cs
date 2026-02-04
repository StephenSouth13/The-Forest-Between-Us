using UnityEngine;

public class MinimapZoomManager : MonoBehaviour
{
    [Header("Minimap Camera")]
    public Camera minimapCamera;

    [Header("Zoom Settings")]
    public float zoomStep = 5f;        // Mỗi lần bấm + / - thay đổi bao nhiêu
    public float zoomSpeed = 8f;        // Tốc độ lerp mượt
    public float minZoom = 5f;          // Zoom gần nhất (không cho nhỏ hơn)
    public float maxZoom = 30f;         // Zoom xa nhất (không cho lớn hơn)

    private float targetZoom;

    void Start()
    {
        if (minimapCamera == null)
        {
            Debug.LogError("Chưa gán Minimap Camera!");
            return;
        }

        // Lấy zoom hiện tại làm target ban đầu
        targetZoom = minimapCamera.orthographicSize;
    }

    void Update()
    {
        cameraLerp();
    }

    public void cameraLerp()
    {
        minimapCamera.orthographicSize = Mathf.Lerp(
            minimapCamera.orthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }

    // Gọi hàm này khi bấm nút +
    public void ZoomIn()
    {
        targetZoom -= zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    // Gọi hàm này khi bấm nút -
    public void ZoomOut()
    {
        targetZoom += zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }
}

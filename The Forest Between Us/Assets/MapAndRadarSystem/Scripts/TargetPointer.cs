using UnityEngine;

namespace MapAndRadarSystem
{
    public class TargetPointer : MonoBehaviour
    {
        public Texture arrow;
        public float size = 30;
        public GameObject PointedTarget;
        public bool hoverOnScreen = true;
        public float distanceAbove = 20;
        public float blindSpot = 0.5f;
        public float hoverAngle = 270;
        private float halfSize;
        public static TargetPointer Instance;

        private void Awake()
        {
            Instance = this;
        }

        Camera cam;

        void OnGUI()
        {
            if (MapAndRadarManager.Instance.ActorCamera == null || PointedTarget == null || MapAndRadarManager.Instance.Panel_Map.activeSelf) return;
            cam = MapAndRadarManager.Instance.ActorCamera.GetComponent<Camera>();

            if (Event.current.type.Equals(EventType.Repaint))
            {
                halfSize = size / 2;
                float angle = hoverAngle - 180;
                float rad = angle * (Mathf.PI / 180);
                Vector3 arrowPos = cam.transform.right * Mathf.Cos(rad) + cam.transform.up * Mathf.Sin(rad);
                Vector3 worldPos = PointedTarget.transform.position + (arrowPos * distanceAbove);
                Vector3 pos = cam.WorldToViewportPoint(worldPos);

                if (pos.z < 0)
                {
                    pos.x *= -1;
                    pos.y *= -1;
                }

                if (pos.z > 0 || (pos.z < 0))
                {
                    var newX = pos.x * cam.pixelWidth;
                    var newY = cam.pixelHeight - pos.y * cam.pixelHeight;
                    if (pos.z < 0 || (newY < 0 || newY > cam.pixelHeight || newX < 0 || newX > cam.pixelWidth))
                    {
                        float a = CalculateAngle(cam.pixelWidth / 2, cam.pixelHeight / 2, newX, newY);
                        Vector2 coord = ProjectToEdge(newX, newY);
                        GUIUtility.RotateAroundPivot(a, coord);
                        float x = coord.x - halfSize;
                        float y = coord.y - halfSize;
                        if (x < 0) x = 0;
                        if (x > Screen.width) x = Screen.width - size;
                        Graphics.DrawTexture(new Rect(x, y, size, size), arrow);
                        GUIUtility.RotateAroundPivot(-a, coord);
                    }
                    else if (hoverOnScreen)
                    {
                        float nh = Mathf.Sin(rad) * size;
                        float nw = Mathf.Cos(rad) * size;
                        GUIUtility.RotateAroundPivot(-angle + 180, new Vector2(newX + nw, newY - nh));
                        Graphics.DrawTexture(new Rect(newX + nw, newY - nh - halfSize, size, size), arrow);
                        GUIUtility.RotateAroundPivot(angle - 180, new Vector2(newX + nw, newY - nh));
                    }
                }
            }
        }

        float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            var xDiff = x2 - x1;
            var yDiff = y2 - y1;
            var rad = Mathf.Atan(yDiff / xDiff);
            var deg = rad * 180 / Mathf.PI;

            if (xDiff < 0)
            {
                deg += 180;
            }

            return deg;
        }

        Vector2 ProjectToEdge(float x2, float y2)
        {
            float xDiff = x2 - (cam.pixelWidth / 2);
            float yDiff = y2 - (cam.pixelHeight / 2);

            Vector2 coord = new Vector2(0, 0);
            float ratio;

            if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
            {
                ratio = (cam.pixelWidth / 2 - halfSize) / Mathf.Abs(xDiff);
                coord.x = xDiff > 0 ? cam.pixelWidth - halfSize : halfSize;
                coord.y = cam.pixelHeight / 2 + yDiff * ratio;
            }
            else
            {
                ratio = (cam.pixelHeight / 2 - halfSize) / Mathf.Abs(yDiff);
                coord.y = yDiff > 0 ? cam.pixelHeight - halfSize : halfSize;
                coord.x = cam.pixelWidth / 2 + xDiff * ratio;
            }

            coord.x = Mathf.Clamp(coord.x, halfSize, cam.pixelWidth - halfSize);
            coord.y = Mathf.Clamp(coord.y, halfSize, cam.pixelHeight - halfSize);

            return coord;
        }
    }
}
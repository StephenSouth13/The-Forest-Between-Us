using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MapAndRadarSystem
{
    public class RadarSystem : MonoBehaviour
    {
        public Camera RadarCamera;
        public static RadarSystem Instance;
        public float RadarDistance;
        public Image Background;
        public Transform PlayerView;
        public RectTransform RadarTarget;
        public RectTransform RadarPoint;
        public Transform Root;
        public List<RadarTargetType> RadarTypes;
        public Dictionary<GameObject, TargetMap> TargetList = new Dictionary<GameObject, TargetMap>();


        private void Awake()
        {
            Instance = this;
        }

        float lastTimeCheck = 0;
        private void Update()
        {
            if(Time.time > lastTimeCheck + 1f)
            {
                lastTimeCheck = Time.time;
                foreach (var target in TargetList)
                {
                    if (target.Key == null)
                    {
                        target.Value.TargetPoint.gameObject.SetActive(false);
                        TargetList.Remove(target.Key);
                        break;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (MapAndRadarManager.Instance.Actor == null) return;
            RadarCamera.transform.eulerAngles = new Vector3(90f, MapAndRadarManager.Instance.Actor.eulerAngles.y, 0f);
            RadarDraw();
        }
        public void AddTarget(RadarItem item)
        {
            if (item == null) return;
            AddTarget(item.gameObject, item.TargetType);
        }

        private void AddTarget(GameObject item, RadarTargetType type)
        {
            if (item == null) return;
            if (TargetList.ContainsKey(item)) return;
            var targetInfo = CreateEnemyInfo(type);
            TargetList.Add(item, targetInfo);
        }

        public void RemoveTarget(GameObject item)
        {
            foreach (var target in TargetList)
            {
                if (target.Key != null && target.Key == item)
                {
                    target.Value.TargetPoint.gameObject.SetActive(false);
                    TargetList.Remove(item);
                    break;
                }
            }
        }

        private TargetMap CreateEnemyInfo(RadarTargetType type)
        {
            var enemyInfo = new TargetMap
            {
                TargetPoint = (RectTransform)Instantiate(RadarPoint, new Vector3(0, 0, 0), Quaternion.identity)
            };
            enemyInfo.TargetPoint.transform.SetParent(Root);
            enemyInfo.TargetPoint.localPosition = new Vector3(0, 0, 0);
            enemyInfo.TargetPoint.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            enemyInfo.TargetPoint.GetComponent<Image>().sprite = type.Sprite;
            return enemyInfo;
        }
      
        private void RadarDraw()
        {
            PlayerView.localRotation = Quaternion.AngleAxis(MapAndRadarManager.Instance.ActorCamera.eulerAngles.y - MapAndRadarManager.Instance.Actor.eulerAngles.y, new Vector3(0, 0, -1));
            RadarCamera.rect = new Rect(0, 0, 200f / Screen.width, 200f / Screen.height);

            foreach (var enemy in TargetList)
            {
                if(enemy.Key != null)
                {
                    RadarCamera.transform.position = new Vector3(MapAndRadarManager.Instance.Actor.position.x, enemy.Key.transform.position.y + RadarDistance, MapAndRadarManager.Instance.Actor.position.z);
                    if (Vector3.Distance(RadarCamera.transform.position, enemy.Key.transform.position) < RadarDistance + RadarDistance * 0.1f)
                    {
                        var screenPos = RadarCamera.WorldToScreenPoint(enemy.Key.transform.position);
                        var size = Background.rectTransform.sizeDelta.x;
                        screenPos = new Vector3((screenPos.x - size) / 2, (screenPos.y - size) / 2);
                        enemy.Value.TargetPoint.localPosition = new Vector3(screenPos.x, screenPos.y);

                        enemy.Value.TargetPoint.gameObject.SetActive(true);

                    }
                    else
                    {
                        var olRot = RadarCamera.transform.eulerAngles;
                        RadarCamera.transform.LookAt(enemy.Key.transform.position);
                        RadarCamera.transform.eulerAngles = olRot;
                        enemy.Value.TargetPoint.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public class TargetMap
    {
        public RectTransform TargetPoint;
    }
}

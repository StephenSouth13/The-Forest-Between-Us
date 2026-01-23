using UnityEngine;

namespace MapAndRadarSystem
{
    public class RadarItem : MonoBehaviour
    {
        public RadarTargetType TargetType;

        void Start()
        {
            if (RadarSystem.Instance != null)
                RadarSystem.Instance.AddTarget(this);
        }
    }
}
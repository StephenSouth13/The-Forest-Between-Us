using UnityEngine;

namespace MapAndRadarSystem
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RadarTargetType", order = 1)]
    public class RadarTargetType : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;
    }
}
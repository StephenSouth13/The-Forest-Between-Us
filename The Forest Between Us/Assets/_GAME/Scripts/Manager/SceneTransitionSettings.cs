using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scene Transition Settings", menuName = "Forest Between Us/Scene Transition Settings")]
public class SceneTransitionSettings : ScriptableObject
{
    [Header("Fade")]
    public Color fadeColor = Color.black;
    public float fadeDuration = 0.6f;
    public AnimationCurve fadeEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Cinematic Bars")]
    public bool useCinematicBars = true;
    [Range(0f, 0.25f)] public float barsHeightRatio = 0.1f;
    public float barsDuration = 0.5f;

    [Header("Loading Screen")]
    public float minDisplaySeconds = 1.5f;
    public Sprite[] defaultBackgrounds;
    public List<SceneBackgroundOverride> sceneBackgrounds = new List<SceneBackgroundOverride>();

    [Header("Tips")]
    [TextArea] public List<string> tips = new List<string>();
    public float tipInterval = 3f;

    public Sprite GetBackgroundFor(string sceneName)
    {
        foreach (SceneBackgroundOverride entry in sceneBackgrounds)
        {
            if (entry.sceneName == sceneName && entry.background != null) return entry.background;
        }

        if (defaultBackgrounds != null && defaultBackgrounds.Length > 0)
        {
            return defaultBackgrounds[Random.Range(0, defaultBackgrounds.Length)];
        }

        return null;
    }

    public string GetRandomTip()
    {
        if (tips == null || tips.Count == 0) return "";
        return tips[Random.Range(0, tips.Count)];
    }
}

[System.Serializable]
public class SceneBackgroundOverride
{
    public string sceneName;
    public Sprite background;
}

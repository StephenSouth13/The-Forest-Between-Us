using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class ForestEnvironmentFixerTool
{
    [MenuItem("Tools/Forest Between Us/🔍 Quét & Sửa Lỗi Scene Forest_EnvironmentSample", false, 1)]
    public static void ScanAndRepairForestSampleScene()
    {
        string targetScenePath = "Assets/Scenes/Forest_EnvironmentSample.unity";

        // 1. Mở Scene nếu chưa mở
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.path != targetScenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                activeScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
            }
            else
            {
                Debug.LogWarning("⚠️ Đã hủy quét vì chưa lưu Scene hiện tại.");
                return;
            }
        }

        Debug.Log("🔍 [ForestFixer] Bắt đầu quét & sửa toàn bộ lỗi trên Scene Forest_EnvironmentSample...");

        int fixedShadersCount = 0;
        int fixedLightsCount = 0;
        int fixedOverlaysCount = 0;

        // -------------------------------------------------------------
        // STEP 1: FIX BLACK OVERLAYS & UI CANVAS PANELS
        // -------------------------------------------------------------
        CanvasGroup[] canvasGroups = Object.FindObjectsByType<CanvasGroup>(FindObjectsSortMode.None);
        foreach (var cg in canvasGroups)
        {
            if (cg.gameObject.name.Contains("Fade") || cg.gameObject.name.Contains("Death") || cg.gameObject.name.Contains("Transition") || cg.gameObject.name.Contains("Loading"))
            {
                cg.alpha = 0f;
                cg.blocksRaycasts = false;
                cg.interactable = false;
                fixedOverlaysCount++;
            }
        }

        GameObject deathPanel = GameObject.Find("PlayerDeath_Panel");
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
            fixedOverlaysCount++;
        }

        // -------------------------------------------------------------
        // STEP 2: FIX LIGHTING & ATMOSPHERE (Mặt trời & Sương mù)
        // -------------------------------------------------------------
        Light sun = RenderSettings.sun;
        if (sun == null)
        {
            Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    sun = l;
                    RenderSettings.sun = l;
                    break;
                }
            }
        }

        if (sun == null)
        {
            GameObject sunGO = new GameObject("Directional Light", typeof(Light));
            sun = sunGO.GetComponent<Light>();
            sun.type = LightType.Directional;
            RenderSettings.sun = sun;
            Undo.RegisterCreatedObjectUndo(sunGO, "Create Directional Light");
            fixedLightsCount++;
        }

        // Setup Mặt Trời Sáng Đẹp
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        sun.intensity = 1.35f;
        sun.color = new Color(1.0f, 0.95f, 0.85f);
        sun.shadows = LightShadows.Soft;

        // Ambient Lighting & Fog
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientSkyColor = new Color(0.6f, 0.7f, 0.85f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.5f, 0.6f);
        RenderSettings.ambientGroundColor = new Color(0.25f, 0.2f, 0.15f);
        RenderSettings.ambientIntensity = 1.1f;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = new Color(0.55f, 0.68f, 0.78f);
        RenderSettings.fogDensity = 0.003f;

        // -------------------------------------------------------------
        // STEP 3: FIX MAGENTA (PINK) MATERIALS & SHADERS
        // -------------------------------------------------------------
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null) urpLitShader = Shader.Find("Standard");

        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        HashSet<Material> checkedMaterials = new HashSet<Material>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials;
            bool modified = false;

            for (int i = 0; i < mats.Length; i++)
            {
                Material m = mats[i];
                if (m == null) continue;

                if (!checkedMaterials.Contains(m))
                {
                    checkedMaterials.Add(m);

                    // Kiểm tra Shader bị gãy / missing / Hidden/InternalErrorShader
                    if (m.shader == null || m.shader.name == "Hidden/InternalErrorShader" || m.shader.name == "" || m.shader.name.Contains("Error"))
                    {
                        if (urpLitShader != null)
                        {
                            m.shader = urpLitShader;
                            EditorUtility.SetDirty(m);
                            fixedShadersCount++;
                            modified = true;
                        }
                    }
                }
            }

            if (modified)
            {
                r.sharedMaterials = mats;
            }
        }

        // -------------------------------------------------------------
        // STEP 4: SETUP MANAGERS & PLAYER
        // -------------------------------------------------------------
        GameObject managersObj = GameObject.Find("GameManagers");
        if (managersObj == null)
        {
            managersObj = new GameObject("GameManagers");
            Undo.RegisterCreatedObjectUndo(managersObj, "Create GameManagers");
        }

        EnsureComponent<DayManager>(managersObj);
        EnsureComponent<PlayerStatsManager>(managersObj);
        EnsureComponent<PlayerRespawnManager>(managersObj);
        EnsureComponent<GameDirector>(managersObj);
        EnsureComponent<FoodSpoilageManager>(managersObj);
        EnsureComponent<PlayerDiseaseManager>(managersObj);

        DayManager dm = managersObj.GetComponent<DayManager>();
        if (dm != null && dm.directionalLight == null)
        {
            dm.directionalLight = sun;
        }

        // Reset Player Vitals
        if (PlayerStatsManager.instance != null)
        {
            PlayerStatsManager.instance.ResetVitalsToFull();
        }

        // Clear player prefs saved death state
        PlayerPrefs.SetFloat("Player_Health", 100f);
        PlayerPrefs.Save();

        // Save Scene
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log($"✅ [ForestFixer] Hoàn tất Quét & Sửa Lỗi Scene Forest_EnvironmentSample!\n" +
                  $"• Đã sửa {fixedShadersCount} Material tím/gãy Shader.\n" +
                  $"• Đã tối ưu {fixedLightsCount + 1} Ánh sáng & Sương mù.\n" +
                  $"• Đã ẩn {fixedOverlaysCount} Canvas màn hình đen bị kẹt.");

        EditorUtility.DisplayDialog("Quét & Sửa Lỗi Hoàn Tất",
            $"🌲 Scene Forest_EnvironmentSample đã được khôi phục thành công!\n\n" +
            $"• Sửa Material tím/gãy Shader: {fixedShadersCount}\n" +
            $"• Ánh sáng & Sương mù: Sáng đẹp (Daylight)\n" +
            $"• Xả đè Màn hình đen UI: Hoàn tất\n" +
            $"• Đã nạp đủ Bộ khung Managers & Player Vitals (100 HP)\n\n" +
            $"Bạn có thể nhấn PLAY để trải nghiệm ngay!", "Tuyệt Vời");
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Singleton persistent qua các scene. Gọi SceneTransitionManager.Instance.LoadScene("TenScene")
// để chuyển cảnh có fade + màn hình loading (ảnh nền, thanh tiến trình, tip xoay vòng, thanh cinematic).
// Instance tự nạp prefab "SceneTransitionManager" từ 1 thư mục Resources (xem SceneTransitionSetupTool).
[RequireComponent(typeof(CanvasGroup))]
public class SceneTransitionManager : MonoBehaviour
{
    static SceneTransitionManager instance;

    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance != null) return instance;

            GameObject prefab = Resources.Load<GameObject>("SceneTransitionManager");
            if (prefab == null)
            {
                Debug.LogError("Không tìm thấy prefab 'SceneTransitionManager' trong thư mục Resources. " +
                                "Chạy Tools > Forest Between Us > Setup Scene Transition trong Unity trước.");
                return null;
            }

            GameObject go = Instantiate(prefab);
            go.name = "SceneTransitionManager";
            instance = go.GetComponent<SceneTransitionManager>();
            DontDestroyOnLoad(go);
            return instance;
        }
    }

    [Header("Default Settings")]
    public SceneTransitionSettings settings;

    [Header("UI References")]
    public CanvasGroup fadeGroup;
    public Image colorBackdrop;
    public Image background;
    public Image vignette;
    public Image spinner;
    public Image progressFill;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI tipText;
    public RectTransform topBar;
    public RectTransform bottomBar;

    bool isTransitioning;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeGroup == null) fadeGroup = GetComponent<CanvasGroup>();
        if (spinner != null && spinner.sprite == null) spinner.sprite = GenerateRingSprite();
        if (vignette != null && vignette.sprite == null) vignette.sprite = GenerateVignetteSprite();

        SetHidden();
    }

    // Gradient tối dần ra rìa màn hình để ảnh nền loading trông điện ảnh hơn, không cần asset ảnh
    static Sprite GenerateVignetteSprite(int size = 256, float maxAlpha = 0.65f)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = Vector2.Distance(Vector2.zero, center);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / maxDist;
                float alpha = Mathf.SmoothStep(0f, maxAlpha, dist);
                texture.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    // Tạo sprite hình khuyên (ring) bằng code để spinner không phụ thuộc asset ảnh nào cả
    static Sprite GenerateRingSprite(int size = 128, float innerRatio = 0.72f)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float outerRadius = size / 2f - 2f;
        float innerRadius = outerRadius * innerRatio;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = 0f;

                if (dist <= outerRadius && dist >= innerRadius)
                {
                    // dùng góc để chừa 1 khoảng hở nhỏ (spinner kiểu "C" thay vì vòng tròn kín)
                    float angle = Mathf.Atan2(y + 0.5f - center.y, x + 0.5f - center.x) * Mathf.Rad2Deg;
                    if (angle < 0f) angle += 360f;
                    alpha = angle > 40f ? 1f : Mathf.InverseLerp(0f, 40f, angle);
                }

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    public void LoadScene(string sceneName, SceneTransitionSettings overrideSettings = null)
    {
        if (isTransitioning) return;

        StartCoroutine(TransitionRoutine(sceneName, overrideSettings != null ? overrideSettings : settings));
    }

    public void TransitionToScene(string sceneName, SceneTransitionSettings overrideSettings = null)
    {
        LoadScene(sceneName, overrideSettings);
    }

    IEnumerator TransitionRoutine(string sceneName, SceneTransitionSettings s)
    {
        isTransitioning = true;
        fadeGroup.blocksRaycasts = true;
        fadeGroup.interactable = true;

        PrepareLoadingScreen(sceneName, s);

        Coroutine tipRoutine = (s != null && s.tips != null && s.tips.Count > 1) ? StartCoroutine(RotateTips(s)) : null;
        Coroutine spinRoutine = StartCoroutine(SpinLoader());

        // Chạy song song với fade để thanh cinematic thực sự "trượt vào" trong lúc màn hình mờ dần, không bị ẩn rồi mới hiện
        if (s != null && s.useCinematicBars) StartCoroutine(AnimateBars(true, s));
        yield return StartCoroutine(Fade(0f, 1f, s));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float minDisplay = s != null ? s.minDisplaySeconds : 1f;

        while (!op.isDone)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            SetProgress(progress);

            if (op.progress >= 0.9f && elapsed >= minDisplay)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }

        SetProgress(1f);
        yield return null; // để scene mới chạy xong Awake/Start trước khi mở màn ra

        if (s != null && s.useCinematicBars) StartCoroutine(AnimateBars(false, s));
        yield return StartCoroutine(Fade(1f, 0f, s));

        if (tipRoutine != null) StopCoroutine(tipRoutine);
        StopCoroutine(spinRoutine);

        SetHidden();
        isTransitioning = false;
    }

    void PrepareLoadingScreen(string sceneName, SceneTransitionSettings s)
    {
        Color color = s != null ? s.fadeColor : Color.black;
        if (colorBackdrop != null) colorBackdrop.color = color;

        Sprite bg = s != null ? s.GetBackgroundFor(sceneName) : null;
        if (background != null)
        {
            background.sprite = bg;
            background.enabled = bg != null;
        }

        if (tipText != null) tipText.text = s != null ? s.GetRandomTip() : "";
        SetProgress(0f);
    }

    void SetProgress(float progress01)
    {
        if (progressFill != null) progressFill.rectTransform.anchorMax = new Vector2(progress01, 1f);
        if (progressText != null) progressText.text = Mathf.RoundToInt(progress01 * 100f) + "%";
    }

    IEnumerator Fade(float from, float to, SceneTransitionSettings s)
    {
        float duration = s != null ? s.fadeDuration : 0.5f;
        fadeGroup.alpha = from;

        if (duration <= 0f)
        {
            fadeGroup.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            float eased = (s != null && s.fadeEase != null && s.fadeEase.length > 0) ? s.fadeEase.Evaluate(normalized) : normalized;
            fadeGroup.alpha = Mathf.Lerp(from, to, eased);
            yield return null;
        }

        fadeGroup.alpha = to;
    }

    IEnumerator RotateTips(SceneTransitionSettings s)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(s.tipInterval);
            if (tipText != null) tipText.text = s.GetRandomTip();
        }
    }

    IEnumerator SpinLoader()
    {
        if (spinner == null) yield break;

        while (true)
        {
            spinner.rectTransform.Rotate(0f, 0f, -180f * Time.unscaledDeltaTime);
            yield return null;
        }
    }

    IEnumerator AnimateBars(bool show, SceneTransitionSettings s)
    {
        if (topBar == null || bottomBar == null) yield break;

        float targetHeight = show ? Screen.height * s.barsHeightRatio : 0f;
        float startHeight = topBar.sizeDelta.y;
        float duration = Mathf.Max(0.01f, s.barsDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float h = Mathf.Lerp(startHeight, targetHeight, Mathf.Clamp01(t / duration));
            topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, h);
            bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, h);
            yield return null;
        }

        topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, targetHeight);
        bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, targetHeight);
    }

    void SetHidden()
    {
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.interactable = false;
    }
}

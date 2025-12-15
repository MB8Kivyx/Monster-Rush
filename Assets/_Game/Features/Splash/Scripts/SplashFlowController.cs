using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class SplashFlowController : MonoBehaviour
{
    [Header("UI Refs")]
    public RectTransform logoMark;
    public CanvasGroup logoMarkCg;
    public RectTransform logoTitle;
    public CanvasGroup logoTitleCg;
    public CanvasGroup taglineCg;
    public CanvasGroup fadeOverlay;

    [Header("Timings")]
    public float minShowTime = 1.6f; // safe time before allowing skip
    public float markIn = 0.6f;
    public float titleDelay = 0.2f;
    public float titleIn = 0.5f;
    public float hold = 0.6f;
    public float fadeOut = 0.5f;

    [Header("Next Scene")]
    public string nextSceneName = "wave"; // or your main menu scene name

    [Header("Audio")]
    public AudioSource whoosh;

    private Vector3 _markStartScale;
    private bool _canSkip;
    private bool _loading;
    private AsyncOperation _preloadOperation;
    private int _previousTargetFrameRate;
    private bool _restoredFrameRate;

    private void Awake()
    {
        _previousTargetFrameRate = Application.targetFrameRate;
        if (_previousTargetFrameRate != 120)
        {
            Application.targetFrameRate = 120; // smoother animation without affecting gameplay later
        }

        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvas.pixelPerfect = false;

        var scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        var raycaster = GetComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        _markStartScale = logoMark != null ? logoMark.localScale : Vector3.one;

        if (logoMark != null)
        {
            logoMark.localScale = _markStartScale * 0.8f;
        }

        if (logoMarkCg != null)
        {
            logoMarkCg.alpha = 0f;
        }

        if (logoTitle != null)
        {
            logoTitle.localScale = Vector3.one * 0.95f;
        }

        if (logoTitleCg != null)
        {
            logoTitleCg.alpha = 0f;
        }

        if (taglineCg != null)
        {
            taglineCg.alpha = 0f;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
        }
    }

    private void OnDestroy()
    {
        if (!_restoredFrameRate)
        {
            Application.targetFrameRate = _previousTargetFrameRate;
            _restoredFrameRate = true;
        }
    }

    private IEnumerator Start()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            _preloadOperation = SceneManager.LoadSceneAsync(nextSceneName);
            if (_preloadOperation != null)
            {
                _preloadOperation.allowSceneActivation = false;
            }
        }

        if (whoosh != null)
        {
            whoosh.Play();
        }

        if (logoMarkCg != null)
        {
            yield return StartCoroutine(TweenCanvas(logoMarkCg, 0f, 1f, markIn));
        }

        if (logoMark != null)
        {
            yield return StartCoroutine(TweenScale(logoMark, _markStartScale * 0.8f, _markStartScale, markIn));
        }

        yield return new WaitForSeconds(titleDelay);

        if (logoTitleCg != null)
        {
            yield return StartCoroutine(TweenCanvas(logoTitleCg, 0f, 1f, titleIn));
        }

        if (taglineCg != null)
        {
            yield return StartCoroutine(TweenCanvas(taglineCg, 0f, 1f, 0.4f));
        }

        yield return new WaitForSeconds(minShowTime);
        _canSkip = true;

        yield return new WaitForSeconds(hold);
        yield return StartCoroutine(ExitAndActivate(_preloadOperation));
    }

    private void Update()
    {
        if (!_canSkip)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            _canSkip = false;
            StartCoroutine(FastExit());
        }
    }

    private IEnumerator FastExit()
    {
        if (_preloadOperation == null && !string.IsNullOrEmpty(nextSceneName))
        {
            _preloadOperation = SceneManager.LoadSceneAsync(nextSceneName);
            if (_preloadOperation != null)
            {
                _preloadOperation.allowSceneActivation = false;
            }
        }

        yield return StartCoroutine(FadeAllOut(0.35f));

        if (_preloadOperation != null)
        {
            _preloadOperation.allowSceneActivation = true;
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator ExitAndActivate(AsyncOperation op)
    {
        if (_loading || op == null)
        {
            yield break;
        }

        _loading = true;
        yield return StartCoroutine(FadeAllOut(fadeOut));
        op.allowSceneActivation = true;
    }

    private IEnumerator FadeAllOut(float duration)
    {
        Coroutine overlay = null;
        if (fadeOverlay != null)
        {
            overlay = StartCoroutine(TweenCanvas(fadeOverlay, fadeOverlay.alpha, 1f, duration));
        }

        if (logoMarkCg != null)
        {
            yield return StartCoroutine(TweenCanvas(logoMarkCg, logoMarkCg.alpha, 0f, duration));
        }

        if (logoTitleCg != null)
        {
            yield return StartCoroutine(TweenCanvas(logoTitleCg, logoTitleCg.alpha, 0f, duration * 0.9f));
        }

        if (taglineCg != null)
        {
            yield return StartCoroutine(TweenCanvas(taglineCg, taglineCg.alpha, 0f, duration * 0.8f));
        }

        if (overlay != null)
        {
            yield return overlay;
        }
    }

    private IEnumerator TweenCanvas(CanvasGroup canvasGroup, float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = time > 0f ? Mathf.Clamp01(elapsed / time) : 1f;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private IEnumerator TweenScale(RectTransform rectTransform, Vector3 from, Vector3 to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = time > 0f ? Mathf.Clamp01(elapsed / time) : 1f;
            rectTransform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        rectTransform.localScale = to;
    }
}

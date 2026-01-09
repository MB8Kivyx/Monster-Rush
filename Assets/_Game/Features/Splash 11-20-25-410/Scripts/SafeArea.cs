using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class SafeArea : MonoBehaviour
{
    private RectTransform _panel;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void OnEnable()
    {
        if (_panel == null)
        {
            _panel = GetComponent<RectTransform>();
        }

        ApplySafeArea();
    }

    private void Update()
    {
        if (_panel == null)
        {
            return;
        }

        if (Application.isEditor || Screen.orientation != _lastOrientation || Screen.safeArea != _lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        _lastOrientation = Screen.orientation;
        _lastSafeArea = Screen.safeArea;

        var anchorMin = _lastSafeArea.position;
        var anchorMax = _lastSafeArea.position + _lastSafeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _panel.anchorMin = anchorMin;
        _panel.anchorMax = anchorMax;
    }
}

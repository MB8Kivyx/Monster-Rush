using UnityEngine;

/// <summary>
/// Drives the Bubble Tesseract soft-body idle motion and emission pulse without requiring an Animator.
/// </summary>
[DisallowMultipleComponent]
public class BubbleTesseractIdle : MonoBehaviour
{
    [Header("Target & Material")]
    [SerializeField] Transform target;
    [SerializeField] Renderer rend;

    [Header("Breath (scale)")]
    [SerializeField, Range(0f, 0.25f)] float breathAmp = 0.06f;
    [SerializeField] float breathHz = 0.35f;

    [Header("Wobble (rotation)")]
    [SerializeField, Range(0f, 12f)] float wobbleDeg = 3.0f;
    [SerializeField] float wobbleHz = 0.22f;

    [Header("Bulge (shader vertex)")]
    [SerializeField, Range(0f, 0.2f)] float displaceAmp = 0.08f;
    [SerializeField] float bulgePulseHz = 0.55f;

    [Header("Emission Pulse")]
    [SerializeField] Color emissionColor = new Color(0.50f, 0.80f, 0.90f, 1f);
    [SerializeField] float emissionBase = 1.2f;
    [SerializeField] float emissionAmp = 0.9f;
    [SerializeField] float emissionHz = 0.7f;

    static readonly int DisplaceAmpId = Shader.PropertyToID("_DisplaceAmp");
    static readonly int PulseId = Shader.PropertyToID("_Pulse");
    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    static readonly int EmissionStrengthId = Shader.PropertyToID("_EmissionStrength");

    Vector3 baseScale;
    Quaternion baseRotation;
    MaterialPropertyBlock mpb;

    void Reset()
    {
        target = transform;
        rend = GetComponentInChildren<Renderer>();
    }

    void Awake()
    {
        if (!target) target = transform;
        if (!rend) rend = GetComponentInChildren<Renderer>();

        baseScale = target.localScale;
        baseRotation = target.localRotation;
        mpb = new MaterialPropertyBlock();
    }

    void OnEnable()
    {
        ApplyStaticProperties();
    }

    void Update()
    {
        float t = Time.time;

        float breath = 1f + breathAmp * Mathf.Sin(Mathf.PI * 2f * breathHz * t);
        target.localScale = baseScale * breath;

        float rx = wobbleDeg * Mathf.Sin(Mathf.PI * 2f * wobbleHz * t);
        float ry = wobbleDeg * Mathf.Sin(Mathf.PI * 2f * wobbleHz * t + 1.4f);
        float rz = wobbleDeg * Mathf.Sin(Mathf.PI * 2f * wobbleHz * t + 2.2f);
        target.localRotation = baseRotation * Quaternion.Euler(rx, ry, rz);

        float pulse = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * 2f * bulgePulseHz * t);
        float emission = emissionBase + emissionAmp * Mathf.Sin(Mathf.PI * 2f * emissionHz * t);

        if (!rend) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(PulseId, pulse);
        mpb.SetFloat(EmissionStrengthId, emission);
        rend.SetPropertyBlock(mpb);
    }

    void ApplyStaticProperties()
    {
        if (!rend) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        rend.GetPropertyBlock(mpb);
        mpb.SetFloat(DisplaceAmpId, displaceAmp);
        mpb.SetColor(EmissionColorId, emissionColor);
        mpb.SetFloat(EmissionStrengthId, emissionBase);
        rend.SetPropertyBlock(mpb);
    }

    void OnValidate()
    {
        if (rend == null) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();
        ApplyStaticProperties();
    }
}

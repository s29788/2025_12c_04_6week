using UnityEngine;

/// Bobbing tylko warstwy wizualnej.
/// Faza bobbingu rośnie proporcjonalnie do dystansu (prędkości),
/// więc przy wolnym biegu jest wolniej, a przy szybkim – szybciej.
/// Nie opuszcza stóp poniżej „ziemi” (offset >= 0).
public class RunBobbingPro : MonoBehaviour
{
    [Header("Źródła")]
    [Tooltip("Rigidbody2D z obiektu Player (RODZIC).")]
    public Rigidbody2D rb;
    [Tooltip("Animator z obiektu Player (RODZIC) – używany do IsGrounded.")]
    public Animator anim;

    [Header("Parametry ruchu")]
    [Tooltip("Pełna prędkość pozioma gracza (dla normalizacji).")]
    public float fullMoveSpeed = 7f;
    [Tooltip("Minimalna prędkość, od której włącza się bobbing.")]
    public float minSpeedForBob = 0.15f;

    [Header("Kształt bobbingu")]
    [Tooltip("Krzywa 0..1 -> offsetY (0..1). Najwyżej w środku kroku.")]
    public AnimationCurve bobCurve = new AnimationCurve(
        new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f)
    );
    [Tooltip("Ile cykli bobbingu na sekundę przy pełnej prędkości.")]
    public float cyclesPerSecondAtFullSpeed = 4.5f;
    [Tooltip("Maksymalna amplituda bobbingu w jednostkach świata (tylko do góry).")]
    public float amplitudeY = 0.04f;
    [Tooltip("Czas wygładzania powrotu do zera (gdy przestajesz biec).")]
    public float relaxTime = 0.08f;

    [Header("Pixel Art / Anti-jitter")]
    [Tooltip("Pixels Per Unit (PPU) twoich sprite’ów. 0 = bez snapu.")]
    public int pixelsPerUnit = 32;
    [Tooltip("Zaokrąglaj do siatki pikseli (eliminuje shimmer).")]
    public bool snapToPixelGrid = true;

    [Header("Debug")]
    public bool forceGrounded = false; // pomoc przy testach
    public bool alwaysOn = false;      // wymuś bobbing niezależnie od prędkości

    Transform t;
    float baseLocalY;
    float phase;            // 0..1
    float smoothOffsetY;    // do wygładzania powrotu
    float velRef;           // ref do SmoothDamp

    void Awake()
    {
        t = transform;
        baseLocalY = t.localPosition.y;

        if (!rb)   rb   = GetComponentInParent<Rigidbody2D>();
        if (!anim) anim = GetComponentInParent<Animator>();
    }

    void LateUpdate()
    {
        float speed = rb ? Mathf.Abs(rb.linearVelocity.x) : 0f;
        bool grounded = anim ? anim.GetBool("IsGrounded") : true;
        if (forceGrounded) grounded = true;

        // Normalizacja prędkości (0..1)
        float speed01 = fullMoveSpeed > 0f ? Mathf.Clamp01(speed / fullMoveSpeed) : 0f;

        // Warunek aktywacji
        bool active = (alwaysOn || speed > minSpeedForBob) && grounded;

        if (active)
        {
            // Faza rośnie od dystansu (prędkość -> częstotliwość)
            float cyclesPerSec = Mathf.Lerp(0f, cyclesPerSecondAtFullSpeed, speed01);
            phase += cyclesPerSec * Time.deltaTime;
            if (phase > 1f) phase -= Mathf.Floor(phase); // modulo 1

            // Offset z krzywej: tylko do góry (>= 0)
            float curve01 = Mathf.Clamp01(bobCurve.Evaluate(phase));
            float targetOffset = curve01 * amplitudeY;

            // brak wcinki w ziemię: offset >= 0 (tylko dodatni)
            smoothOffsetY = targetOffset; // w fazie biegu trzymamy bez damping
        }
        else
        {
            // płynny powrót do 0, gdy staniemy / skoczymy
            smoothOffsetY = Mathf.SmoothDamp(smoothOffsetY, 0f, ref velRef, relaxTime);
            // powoli zwalniaj też fazę, żeby nie „przeskakiwała” przy wznowieniu
            phase = Mathf.Repeat(phase + Time.deltaTime * 0.2f, 1f);
        }

        // Ustaw pozycję (lokalną) – tylko Y w górę
        float y = baseLocalY + smoothOffsetY;

        // Snap do siatki pikseli (usuwa jitter przy Pixel Perfect)
        if (snapToPixelGrid && pixelsPerUnit > 0)
            y = Mathf.Round(y * pixelsPerUnit) / pixelsPerUnit;

        var lp = t.localPosition;
        t.localPosition = new Vector3(lp.x, y, lp.z);
    }

    // Pomaga ustawić ładną krzywą w edytorze
    void OnValidate()
    {
        if (bobCurve.length == 0)
            bobCurve = new AnimationCurve(
                new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f)
            );
        amplitudeY = Mathf.Max(0f, amplitudeY);
        cyclesPerSecondAtFullSpeed = Mathf.Max(0f, cyclesPerSecondAtFullSpeed);
        minSpeedForBob = Mathf.Max(0f, minSpeedForBob);
        fullMoveSpeed = Mathf.Max(0.01f, fullMoveSpeed);
        if (pixelsPerUnit < 0) pixelsPerUnit = 0;
    }
}

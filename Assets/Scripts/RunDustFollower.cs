using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RunDustFollower : MonoBehaviour
{
    [Header("Refs")]
    public ParticleSystem dustPS;      // przypnij swój Particle System (kurz)
    public Transform groundCheck;      // ten sam co w kontrolerze ruchu
    public Animator anim;              // z Playera (rodzica)
    public SpriteRenderer sr;          // z PlayerVisual (dziecka)

    [Header("Grounding")]
    public LayerMask groundMask;       // ta sama warstwa co w ruchu
    public float raycastDown = 0.8f;   // ile w dół szukać ziemi
    public float groundEpsilon = 0.02f;// ile nad ziemią umieścić kurz

    [Header("Placement")]
    public float backOffset = 0.30f;   // jak bardzo „za” plecami (X)
    public float minSpeed = 0.6f;      // od jakiej prędkości emitować
    public float fullMoveSpeed = 7f;   // Twoje moveSpeed

    [Header("Velocity (w tył i lekko w górę)")]
    public float backVel = 0.6f;       // prędkość cząstek w tył
    public float upVel = 0.2f;         // lekko w górę

    [Header("Pixel-art")]
    public int pixelsPerUnit = 32;     // 0 = bez snapu
    public bool snapToPixelGrid = true;

    Rigidbody2D rb;
    Bounds colBounds;
    Collider2D bodyCol;
    ParticleSystem.EmissionModule em;
    ParticleSystem.MainModule main;
    ParticleSystem.VelocityOverLifetimeModule vel;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        if (!sr)   sr   = GetComponentInChildren<SpriteRenderer>(true);
        bodyCol = GetComponent<Collider2D>();

        if (!dustPS)
        {
            Debug.LogError("RunDustFollower: przypnij ParticleSystem (dustPS) – to będzie Twój kurz.");
            enabled = false; return;
        }

        // wymuś World space i skonfiguruj moduły
        main = dustPS.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        em = dustPS.emission;
        em.rateOverTime = 0f; // sterujemy kodem

        vel = dustPS.velocityOverLifetime;
        vel.enabled = true;
    }

    void LateUpdate()
    {
        if (!dustPS) return;

        bool grounded = anim ? anim.GetBool("IsGrounded") : true;
        float speed = Mathf.Abs(rb.linearVelocity.x);

        // 1) Ustal kierunek patrzenia
        bool facingLeft = (sr && sr.flipX);
        float dir = facingLeft ? -1f : 1f;

        // 2) Podstawowa pozycja emitera: „za” plecami, przy stopie
        Vector3 basePos = (groundCheck ? groundCheck.position : transform.position);
        basePos.x += -dir * backOffset;

        // 3) Raycast w dół, aby przykleić Y do ziemi
        Vector3 rayOrigin = basePos + Vector3.up * 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDown, groundMask);

        float y;
        if (hit)
            y = hit.point.y + groundEpsilon;
        else
        {
            // fallback na dolną krawędź kolidera, jeśli ray nie trafi
            if (bodyCol != null)
            {
                colBounds = bodyCol.bounds;
                y = colBounds.min.y + groundEpsilon;
            }
            else
            {
                y = basePos.y - 0.05f; // ostatnia deska ratunku, minimalnie pod groundCheck
            }
        }

        // Snap do siatki pikseli (opcjonalnie)
        if (snapToPixelGrid && pixelsPerUnit > 0)
            y = Mathf.Round(y * pixelsPerUnit) / pixelsPerUnit;

        // 4) Ustaw pozycję emitera
        Vector3 pos = new Vector3(basePos.x, y, dustPS.transform.position.z);
        dustPS.transform.position = pos;

        // 5) Ustaw prędkość cząstek: zawsze „w tył” + lekko w górę
        vel.x = new ParticleSystem.MinMaxCurve(-dir * backVel);
        vel.y = new ParticleSystem.MinMaxCurve(upVel);

        // 6) Steruj emisją – tylko gdy biegniesz po ziemi
        if (grounded && speed >= minSpeed)
        {
            float t = Mathf.InverseLerp(minSpeed, fullMoveSpeed, speed);
            em.rateOverTime = Mathf.Lerp(8f, 28f, t); // zagęszczenie wraz z prędkością
            if (!dustPS.isPlaying) dustPS.Play();
        }
        else
        {
            em.rateOverTime = 0f;
            // nie Stop(), bo World simulation – pozwól dokończyć żyjące cząstki
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.cyan;
        Vector3 o = (sr && sr.flipX ? -1f : 1f) * Vector3.right * backOffset;
        Vector3 p = (groundCheck ? groundCheck.position : transform.position) + o + Vector3.up * 0.25f;
        Gizmos.DrawLine(p, p + Vector3.down * raycastDown);
    }
#endif
}

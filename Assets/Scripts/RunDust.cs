using UnityEngine;

public class RunDust : MonoBehaviour
{

    public LayerMask groundLayer;          // przypnij tę samą warstwę co w kontrolerze gracza
    public float backOffset   = 0.30f;     // jak bardzo "za" graczem (X)
    public float groundOffset = 0.02f;     // jak wysoko nad ziemią (Y)
    public float groundRayDistance = 0.6f; // jak daleko szukamy ziemi w dół


    [Header("Refs")]
    public Rigidbody2D rb;         // Player (rodzic)
    public Animator anim;          // Player (rodzic) – używa IsGrounded
    public SpriteRenderer sr;      // z PlayerVisual (do kierunku)
    public Transform spawnPoint;   // np. groundCheck
    public GameObject dustPrefab;  // DustPoof

    [Header("Tuning")]
    public float minSpeed = 0.6f;      // od jakiej prędkości pojawia się kurz
    public float baseInterval = 0.12f; // odstęp między poofami przy pełnej prędkości
    public float fullMoveSpeed = 7f;   // Twoje moveSpeed z kontrolera

    float timer;

    void Awake()
    {
        if (!rb)   rb   = GetComponentInParent<Rigidbody2D>();
        if (!anim) anim = GetComponentInParent<Animator>();
        if (!sr)   sr   = GetComponentInChildren<SpriteRenderer>(true);
        if (!spawnPoint) spawnPoint = transform; // awaryjnie
    }

    void Update()
    {
        bool grounded = anim ? anim.GetBool("IsGrounded") : true;
        float speed = rb ? Mathf.Abs(rb.linearVelocity.x) : 0f;

        if (!grounded || speed < minSpeed || !dustPrefab) { timer = 0f; return; }

        // im szybciej biegniesz, tym częściej poofy
        float speed01 = Mathf.Clamp01(fullMoveSpeed > 0 ? speed / fullMoveSpeed : 0f);
        float freq = Mathf.Lerp(0.6f, 1.6f, speed01); // mnożnik tempa
        timer -= Time.deltaTime * freq;

        if (timer <= 0f)
        {
            SpawnPoof();
            timer = baseInterval;
        }
    }

    public void SpawnPoof()
    {
        if (!dustPrefab || !spawnPoint) return;

        // 1) Kierunek patrzenia
        bool facingLeft = (sr && sr.flipX);
        float dir = facingLeft ? -1f : 1f;

        // 2) Bazowa pozycja: za graczem w osi X
        Vector3 pos = spawnPoint.position + new Vector3(-dir * backOffset, 0f, 0f);

        // 3) Raycast w dół, żeby "przykleić" kurz do ziemi (tuż nad)
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector3.up * 0.2f, Vector2.down, groundRayDistance, groundLayer);
        if (hit)
        {
            pos.y = hit.point.y + groundOffset;   // tuż nad ziemią
        }
        else
        {
            // fallback gdy nie trafimy — bierzemy wysokość spawnPoint i lekko obniżamy
            pos.y = spawnPoint.position.y - groundOffset;
        }

        // (opcjonalnie) snap do siatki pikseli, jeśli używasz Pixel Perfect (PPU=32)
        // float ppu = 32f; pos.y = Mathf.Round(pos.y * ppu) / ppu;

        // 4) Tworzymy PS w trybie World
        var go = Instantiate(dustPrefab, pos, Quaternion.identity);
        var ps = go.GetComponent<ParticleSystem>();
        if (!ps) return;

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // 5) Ustaw kształt emisji tak, by leciało ZA postacią
        var shape = ps.shape;
        shape.enabled   = true;
        shape.shapeType = ParticleSystemShapeType.Cone; // lub Box
        shape.angle     = 12f;
        shape.radius    = 0.05f;

        // Emisja w tył: dla patrzenia w lewo obrót 0°, dla w prawo 180° po Y
        // (bo "tył" względem świata to -X dla patrzenia w prawo i +X dla w lewo)
        shape.rotation = new Vector3(0f, facingLeft ? 0f : 180f, 0f);

        // 6) Delikatne "pchnięcie" cząstek w tył i odrobinę w górę
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.x = new ParticleSystem.MinMaxCurve(-dir * 0.6f); // w tył względem kierunku biegu
        vel.y = new ParticleSystem.MinMaxCurve(0.2f);        // leciutko w górę
    }


}

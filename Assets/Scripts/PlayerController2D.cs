using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 7f;

    [Header("Skok (variable height)")]
    public float jumpForce = 12f;            // impuls startowy skoku
    public float maxJumpHoldTime = 0.15f;    // ile możesz "dokarmiać" skok trzymając spację (s)
    public float jumpHoldForce = 30f;        // siła podtrzymania podczas hold (Force w czasie)
    public float jumpCutMultiplier = 0.5f;   // ucięcie skoku przy puszczeniu (0..1)
    public float fallGravityMultiplier = 1.6f;     // mocniejsze opadanie
    public float lowJumpGravityMultiplier = 1.2f;  // gdy nie trzymasz skoku wznosząc się

    [Header("Ground check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wizual (PRZYPNIJ z PlayerVisual)")]
    [SerializeField] private Animator anim;           // <- przypnij ręcznie Animator z PlayerVisual
    [SerializeField] private SpriteRenderer sr;       // <- przypnij ręcznie SpriteRenderer z PlayerVisual

    private Rigidbody2D rb;
    private float moveInput;   // -1..1
    private bool wantJump;
    private bool isGrounded;

    // --- zmienne dla variable jump ---
    private bool isJumping;          // jesteśmy w fazie skoku (po starcie)
    private float jumpHoldTimer;     // ile jeszcze podtrzymania zostało
    private float defaultGravity;    // zapamiętana grawitacja
    private bool prevGrounded;       // do wykrycia lądowania (reset triggerów)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // awaryjne wyszukanie, jeśli zapomniałeś przypiąć
        if (!anim)
            anim = transform.Find("PlayerVisual")?.GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);
        if (!sr)
            sr = transform.Find("PlayerVisual")?.GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);

        defaultGravity = rb.gravityScale;
    }

    void Start()
    {
        // TWARDY CHECK: jeśli Animator jest, ale nie ma kontrolera – wyłączamy i logujemy
        if (!anim)
        {
            Debug.LogError("PlayerController2D: Brak referencji do Animator (przypnij z PlayerVisual). Wyłączam skrypt.");
            enabled = false; return;
        }
        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError($"PlayerController2D: Animator '{anim.name}' nie ma przypiętego Controller. Wyłączam skrypt.");
            enabled = false; return;
        }
        if (!sr)
        {
            Debug.LogError("PlayerController2D: Brak referencji do SpriteRenderer (przypnij z PlayerVisual). Wyłączam skrypt.");
            enabled = false; return;
        }
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("IsGrounded", isGrounded);

        if (!wasGrounded && isGrounded)
        {
            // wylądowaliśmy -> czyść wybrane triggery
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("AttackAir");
            anim.ResetTrigger("AttackGround");
        }

        var kb = Keyboard.current;
        moveInput = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  moveInput -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput += 1f;

            // start skoku (krawędź naciśnięcia i tylko na ziemi)
            if (kb.spaceKey.wasPressedThisFrame && isGrounded)
                wantJump = true;

            // UCIĘCIE skoku: gdy puścisz spację podczas wznoszenia
            if (kb.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
                jumpHoldTimer = 0f;   // kończymy podtrzymanie
                isJumping = false;
            }
        }

        if (moveInput != 0f)
            sr.flipX = moveInput < 0f;

        prevGrounded = wasGrounded;
    }

    void FixedUpdate()
    {
        // --- GRUNT ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- RUCH POZIOMY ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- SKOK (start) ---
        if (wantJump && isGrounded)
        {
            anim.SetTrigger("Jump");

            // impuls startowy
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            isJumping = true;
            jumpHoldTimer = maxJumpHoldTime;
        }
        wantJump = false;

        // --- PODTRZYMANIE skoku (hold) ---
        bool holdingJump = Keyboard.current?.spaceKey.isPressed ?? false;
        if (isJumping && holdingJump && jumpHoldTimer > 0f && rb.linearVelocity.y > 0f)
        {
            rb.AddForce(Vector2.up * jumpHoldForce * Time.fixedDeltaTime, ForceMode2D.Force);
            jumpHoldTimer -= Time.fixedDeltaTime;
        }
        // koniec fazy hold, gdy zaczynasz spadać lub puściłeś
        if (rb.linearVelocity.y <= 0f || !holdingJump)
            isJumping = false;

        // --- MODYFIKACJA GRAWITACJI dla lepszego feelingu ---
        if (rb.linearVelocity.y < -0.01f)
        {
            // szybciej opadaj
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0.01f && !holdingJump)
        {
            // jeśli wznosisz się, ale nie trzymasz skoku – szybciej "gaśnie"
            rb.gravityScale = defaultGravity * lowJumpGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }

        // --- PARAMETRY ANIMATORA ---
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("YVelocity", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

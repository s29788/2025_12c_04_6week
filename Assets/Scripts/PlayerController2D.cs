using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 7f;

    [Header("Skok (variable height)")]
    public float jumpForce = 12f;            // impuls startowy skoku
    public float maxJumpHoldTime = 0.15f;    // jak długo trzymanie zwiększa skok
    public float jumpHoldForce = 30f;        // siła podtrzymania podczas hold (Force)
    public float jumpCutMultiplier = 0.5f;   // ucięcie skoku przy puszczeniu klawisza
    public float fallGravityMultiplier = 1.6f;
    public float lowJumpGravityMultiplier = 1.2f;

    [Header("Ground check (3 kółka)")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.18f;        // promień kółka
    public float groundCheckSideOffset = 0.25f;    // jak daleko od środka są lewe/prawe kółko
    public LayerMask groundLayer;

    [Header("Wizual (PRZYPNIJ z PlayerVisual)")]
    [SerializeField] private Animator anim;           // przypnij Animator z PlayerVisual
    [SerializeField] private SpriteRenderer sr;       // przypnij SpriteRenderer z PlayerVisual

    [Header("Coyote time")]
    public float coyoteTime = 0.08f; // 80 ms na spóźniony skok
    private float coyoteTimer;

    [Header("In-Game UI")]
    [SerializeField] private UIManager uiManager;

    private Rigidbody2D rb;
    private float moveInput;
    private bool wantJump;
    private bool isGrounded;

    // variable jump
    private bool isJumping;
    private float jumpHoldTimer;
    private float defaultGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!anim)
            anim = transform.Find("PlayerVisual")?.GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);
        if (!sr)
            sr = transform.Find("PlayerVisual")?.GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);

        defaultGravity = rb.gravityScale;
    }

    void Start()
    {
        if (!anim)
        {
            Debug.LogError("PlayerController2D: brak Animator (PlayerVisual).");
            enabled = false;
            return;
        }
        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator nie ma Controller.");
            enabled = false;
            return;
        }
        if (!sr)
        {
            Debug.LogError("PlayerController2D: brak SpriteRenderer (PlayerVisual).");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = CheckGrounded();
        anim.SetBool("IsGrounded", isGrounded);

        // coyote timer – odnawiamy, gdy stoimy na ziemi
        if (isGrounded) coyoteTimer = coyoteTime;
        else            coyoteTimer -= Time.deltaTime;

        if (!wasGrounded && isGrounded)
        {
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

            // skok – pozwól zarówno na ziemi, jak i w coyote time
            if (kb.spaceKey.wasPressedThisFrame && (isGrounded || coyoteTimer > 0f))
                wantJump = true;

            // ucięcie skoku
            if (kb.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
                jumpHoldTimer = 0f;
                isJumping = false;
            }
        }

        if (moveInput != 0f)
            sr.flipX = moveInput < 0f;
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        // ruch poziomy
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // start skoku (ziemia lub coyote)
        if (wantJump && (isGrounded || coyoteTimer > 0f))
        {
            anim.SetTrigger("Jump");

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            isJumping = true;
            jumpHoldTimer = maxJumpHoldTime;

            coyoteTimer = 0f; // nie skaczemy wielokrotnie z jednego „coyote”
        }
        wantJump = false;

        // podtrzymanie skoku
        bool holdingJump = Keyboard.current?.spaceKey.isPressed ?? false;
        if (isJumping && holdingJump && jumpHoldTimer > 0f && rb.linearVelocity.y > 0f)
        {
            rb.AddForce(Vector2.up * jumpHoldForce * Time.fixedDeltaTime, ForceMode2D.Force);
            jumpHoldTimer -= Time.fixedDeltaTime;
        }
        if (rb.linearVelocity.y <= 0f || !holdingJump)
            isJumping = false;

        // modyfikacja grawitacji
        if (rb.linearVelocity.y < -0.01f)
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        else if (rb.linearVelocity.y > 0.01f && !holdingJump)
            rb.gravityScale = defaultGravity * lowJumpGravityMultiplier;
        else
            rb.gravityScale = defaultGravity;

        // parametry animacji
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("YVelocity", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);
    }

    // 🔵 3× OverlapCircle – środek, lewa, prawa stopa
    bool CheckGrounded()
    {
        if (!groundCheck) return false;

        Vector2 basePos = groundCheck.position;

        // środkowe kółko
        if (Physics2D.OverlapCircle(basePos, groundCheckRadius, groundLayer))
            return true;

        // lewe kółko
        Vector2 left = basePos + Vector2.left * groundCheckSideOffset;
        if (Physics2D.OverlapCircle(left, groundCheckRadius, groundLayer))
            return true;

        // prawe kółko
        Vector2 right = basePos + Vector2.right * groundCheckSideOffset;
        if (Physics2D.OverlapCircle(right, groundCheckRadius, groundLayer))
            return true;

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;

        Vector3 basePos = groundCheck.position;
        Gizmos.DrawWireSphere(basePos, groundCheckRadius);

        Vector3 left = basePos + Vector3.left * groundCheckSideOffset;
        Vector3 right = basePos + Vector3.right * groundCheckSideOffset;

        Gizmos.DrawWireSphere(left, groundCheckRadius);
        Gizmos.DrawWireSphere(right, groundCheckRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                uiManager.coinCount++;
                uiManager.coinText.GetComponent<Animator>().SetTrigger("coinCollected");
            }
            if (other.gameObject.CompareTag("Enemy"))
            {
                uiManager.deathCount++;
                uiManager.deathText.GetComponent<Animator>().SetTrigger("playerDied");
            }
        }
}

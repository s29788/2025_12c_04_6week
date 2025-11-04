using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 7f;

    [Header("Skok")]
    public float jumpForce = 12f;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // awaryjne wyszukanie, jeśli zapomniałeś przypiąć
        if (!anim)
            anim = transform.Find("PlayerVisual")?.GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);
        if (!sr)
            sr = transform.Find("PlayerVisual")?.GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
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

            if (kb.spaceKey.wasPressedThisFrame && isGrounded)
                wantJump = true;
        }

        if (moveInput != 0f)
            sr.flipX = moveInput < 0f;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (wantJump && isGrounded)
        {
            anim.SetTrigger("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        wantJump = false;

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

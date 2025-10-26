using UnityEngine;
using UnityEngine.InputSystem; // nowy Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 7f;

    [Header("Skok")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private float moveInput;   // -1..1
    private bool wantJump;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr  = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("IsGrounded", isGrounded);

        // jeśli dopiero co wylądował -> zresetuj triggery animacji powietrznych
        if (!wasGrounded && isGrounded)
        {
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("AttackAir");
            anim.ResetTrigger("AttackGround");
        }

        // --- INPUT (nowy Input System) ---
        var kb = Keyboard.current;
        moveInput = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  moveInput -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput += 1f;

            if (kb.spaceKey.wasPressedThisFrame && isGrounded)
                wantJump = true;
        }

        // --- FLIP SPRITE ---
        if (moveInput != 0f)
            sr.flipX = moveInput < 0f;
    }

    void FixedUpdate()
    {
        // --- GRUNT ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- RUCH POZIOMY ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- SKOK ---
        if (wantJump && isGrounded)
        {
            // odpalenie triggera skoku (animacja JumpUp)
            anim.SetTrigger("Jump");

            // wyzerowanie pionowej prędkości, by skoki były powtarzalne
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        wantJump = false;

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

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCombat2D : MonoBehaviour
{
    [Header("Hitboxy")]
    public GameObject hitboxSlashGround;
    public GameObject hitboxSlashAir;

    [Header("Odniesienia z PlayerController2D")]
    public Transform groundCheck;       // ten sam co w ruchu
    public LayerMask whatIsGround;      // ten sam co w ruchu
    public float groundCheckRadius = 0.2f;

    [Header("Atak")]
    public float attackCooldown = 0.25f;

    [Header("Air attack behaviour")]
    public float downwardForce = -8f;
    public float airAttackGravityScale = 0.4f;

    [Header("Miecz po ataku")]
    public float swordHoldDuration = 2.0f; // ile sekund po ostatnim ataku ma być „z mieczem”

    private bool canAttack = true;
    private bool isGrounded;
    private bool didAirAttackThisJump;

    private Animator anim;
    private Rigidbody2D rb;
    private float defaultGravityScale;

    private Coroutine swordHoldRoutine; // timer na HasSword

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;

        if (hitboxSlashGround) hitboxSlashGround.SetActive(false);
        if (hitboxSlashAir)    hitboxSlashAir.SetActive(false);

        // na starcie nie trzymamy miecza
        anim.SetBool("HasSword", false);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if (isGrounded) didAirAttackThisJump = false;

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            TryAttack();
    }

    void TryAttack()
    {
        if (!canAttack) return;

        canAttack = false;

        // Każdy atak „odświeża” stan z mieczem
        StartSwordHoldTimer();

        if (isGrounded)
        {
            anim.ResetTrigger("AttackAir");
            anim.SetTrigger("AttackGround");
        }
        else
        {
            if (!didAirAttackThisJump)
            {
                anim.ResetTrigger("AttackGround");
                anim.SetTrigger("AttackAir");
                didAirAttackThisJump = true;
            }
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack() => canAttack = true;

    // --- Animation Events (z klipów) ---
    public void EnableGroundHitbox()
    {
        if (hitboxSlashGround) hitboxSlashGround.SetActive(true);
    }

    public void DisableGroundHitbox()
    {
        if (hitboxSlashGround) hitboxSlashGround.SetActive(false);
    }

    public void EnableAirHitbox()
    {
        if (hitboxSlashAir) hitboxSlashAir.SetActive(true);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, downwardForce);
        rb.gravityScale = airAttackGravityScale;
    }

    public void DisableAirHitbox()
    {
        if (hitboxSlashAir) hitboxSlashAir.SetActive(false);
        rb.gravityScale = defaultGravityScale;
    }

    // --- Timer „miecz po ataku” ---
    void StartSwordHoldTimer()
    {
        // włącz stan miecza
        anim.SetBool("HasSword", true);

        // jeśli timer już leciał, restart
        if (swordHoldRoutine != null) StopCoroutine(swordHoldRoutine);
        swordHoldRoutine = StartCoroutine(SwordHoldCountdown());
    }

    IEnumerator SwordHoldCountdown()
    {
        yield return new WaitForSeconds(swordHoldDuration);
        anim.SetBool("HasSword", false);
        swordHoldRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

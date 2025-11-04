using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDeath : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;          // przypnij w Inspectorze lub znajdziemy po tagu
    public float deathDelay = 0.6f;         // ile czekamy na animację śmierci
    public float invulnerableAfter = 1.0f;  // i-frames po respawnie (opcjonalnie)

    [Header("Do wyłączenia w trakcie śmierci")]
    public MonoBehaviour[] componentsToDisable; // np. PlayerController2D, PlayerCombat2D

    Animator anim;
    Rigidbody2D rb;
    bool isDying = false;
    bool isInvulnerable = false;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb   = GetComponent<Rigidbody2D>();

        // Jeśli nie przypięto respawnu, spróbuj po tagu
        if (respawnPoint == null)
        {
            var rp = GameObject.FindGameObjectWithTag("Respawn");
            if (rp) respawnPoint = rp.transform;
            else Debug.LogError("Brak obiektu z tagiem 'Respawn' (respawnPoint).");
        }
    }

    public void Kill()
    {
        if (isDying || isInvulnerable) return;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        isDying = true;

        // Wyłącz sterowanie/atak
        SetComponentsEnabled(false);

        // Zatrzymaj ruch i odpal animację
        rb.linearVelocity = Vector2.zero;
        anim.ResetTrigger("AttackGround");
        anim.ResetTrigger("AttackAir");
        anim.ResetTrigger("Jump");
        anim.SetTrigger("Die");            // 🔥 dodaj w Animatorze stan "Die"

        // (opcjonalnie) zamroź ruch poziomy
        // rb.constraints |= RigidbodyConstraints2D.FreezePositionX;

        // Poczekaj na animację śmierci
        yield return new WaitForSeconds(deathDelay);

        // Teleport na respawn
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        // Przywróć fizykę i sterowanie
        // rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        SetComponentsEnabled(true);

        anim.ResetTrigger("Die");
        anim.Play("Idle", 0);

        // Krótkie i-frames po respawnie
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableAfter);
        isInvulnerable = false;

        isDying = false;

    }

    void SetComponentsEnabled(bool enabled)
    {
        if (componentsToDisable == null) return;
        foreach (var c in componentsToDisable)
            if (c) c.enabled = enabled;
    }
}

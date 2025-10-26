using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var death = collision.GetComponent<PlayerDeath>();
        if (death != null)
            death.Kill();
        // Nic więcej – brak teleportu tutaj
    }
}

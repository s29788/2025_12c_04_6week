using UnityEngine;

public class Spike : MonoBehaviour
{
    private Transform respawnPoint;

    void Awake()
    {
        GameObject rp = GameObject.FindGameObjectWithTag("Respawn");
        if (rp != null) respawnPoint = rp.transform;
        else Debug.LogError("Brak obiektu z tagiem 'Respawn' w scenie!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && respawnPoint != null)
        {
            var rb = collision.attachedRigidbody;
            if (rb) rb.linearVelocity = Vector2.zero;

            collision.transform.position = respawnPoint.position;
        }
    }
}

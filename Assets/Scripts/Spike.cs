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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && respawnPoint != null)
        {
            var rb = other.attachedRigidbody;
            if (rb) rb.linearVelocity = Vector2.zero;

            other.transform.position = respawnPoint.position;
        }
    }
}

using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;
    private Transform respawnPoint;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        GameObject rp = GameObject.FindGameObjectWithTag("Respawn");
        if (rp != null) respawnPoint = rp.transform;
        else Debug.LogError("Brak obiektu z tagiem 'Respawn' w scenie!");
    }

    private void Start()
    {
        _rb.linearVelocity = transform.right * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && respawnPoint != null)
        {
            var rb = other.attachedRigidbody;
            if (rb) rb.linearVelocity = Vector2.zero;

            other.transform.position = respawnPoint.position;
        }
        
        gameObject.SetActive(false); //Deactivates when hitting any object
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

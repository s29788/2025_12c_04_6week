using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rb.linearVelocity = transform.right * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: player damage script
        if (other.CompareTag("Player"))
            Destroy(other.gameObject);
        
        gameObject.SetActive(false); //Deactivates when hitting any object
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

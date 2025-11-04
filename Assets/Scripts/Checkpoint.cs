using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private Transform respawnPoint;
    [SerializeField] private GameObject checkpointPrefab;
    private Animator _anim;

    void Awake()
    {
        GameObject rp = GameObject.FindGameObjectWithTag("Respawn");
        if (rp != null) respawnPoint = rp.transform;
        else Debug.LogError("Brak obiektu z tagiem 'Respawn' w scenie!");
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && respawnPoint != null)
        {
            respawnPoint.position = checkpointPrefab.transform.position;
            _anim.SetTrigger("SpawnPointSet");
        }
    }
}

using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

     // void OnTriggerEnter2D(Collider2D other)
     // {
     //     if (other.CompareTag("Enemy"))
     //     {
     //         var hp = other.GetComponent<EnemyHealth>();
     //         if (hp) hp.TakeDamage(damage);
     //         else Destroy(other.gameObject);
     //     }
     // }
}

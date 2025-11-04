using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private HealthBar healthBar;

    void Start(){
        healthBar.SetMaxHealth(health);
    }
    
    public void TakeDamage(float damage){
        health -= damage;
        healthBar.SetHealth(health);
        
        if(health <= 0)
            Die();
    }

    public void Die(){
        Destroy(gameObject);
    }
}
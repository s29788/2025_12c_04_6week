using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public HealthBar healthBar;
    private Animator animator;
    
    void Awake(){
        animator = GetComponent<Animator>();
    }

    void Start(){
        healthBar.SetMaxHealth(health);
    }
    
    public void TakeDamage(float damage){
        health -= damage;
        healthBar.SetHealth(health);
        
        if(health <= 0) animator.SetBool("isDead", true);
        else animator.Play("Hit");
        
        GetComponent<EnemyMovement>().Knockback();
    }

    public void Die(){
        Destroy(gameObject);
    }
}
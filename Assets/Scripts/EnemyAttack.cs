using System.Numerics;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage;
    public float attackRadius;
    public Transform attackPoint;
    public LayerMask layer;
    public GameObject player;
    public float attackRange;
    public float attackCooldown;
    private float attackCounter;
    private bool canAttack = true;
    private bool isAttacking = false;
    
    private Animator animator;

    void Awake(){
        animator = gameObject.GetComponent<Animator>();
    }

    void Update(){
        
        // if(UnityEngine.Vector2.Distance(transform.position, player.transform.position) <= attackRange && canAttack){
        //     canAttack = false;
        //     animator.Play("Anticipation");
        //     
        //     Invoke(nameof(ResetAttack), attackCooldown);
        // }
    }

    public void Attack(){
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, layer);
        // if (hit)
        //     hit.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
    }

    public void ResetAttack(){
        canAttack = true;
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

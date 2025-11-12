using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;
    public float speed = 4f;
    private bool isFacingLeft = true;
    private bool isRunning = false;
    
    public GameObject player;
    public float playerAwareness;
    private bool inRange = false;

    public Vector2 knockback = new Vector2(2f, 4f);
    
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;
    
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = pointA.transform;
        animator.SetBool("isRunning", true);
        StartRunning();
    }

    void Update(){
        if(Vector2.Distance(transform.position, player.transform.position) <= playerAwareness)
            inRange = true;
        else inRange = false;
    }
    void FixedUpdate(){
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if(!inRange && isRunning &&isGrounded){
            if(currentPoint == pointB.transform)
                rb.linearVelocity = new Vector2(speed, 0);
            else
                rb.linearVelocity = new Vector2(-speed, 0);
            if(Vector2.Distance(transform.position, currentPoint.position) < 0.2f && currentPoint == pointB.transform){
                Flip();
                currentPoint = pointA.transform;
            }
            if(Vector2.Distance(transform.position, currentPoint.position) < 0.2f && currentPoint == pointA.transform){
                Flip();
                currentPoint = pointB.transform;
            }
            if(isFacingLeft && currentPoint==pointB.transform || !isFacingLeft && currentPoint==pointA.transform) {
                Flip();
            }
        }else if(inRange && isRunning && isGrounded){
            if(player.transform.position.x < transform.position.x){
                if(!isFacingLeft) Flip();
                transform.Translate(Vector2.left * Time.deltaTime * speed);
            }else {
                if(isFacingLeft) Flip();
                transform.Translate(Vector2.right * Time.deltaTime * speed);
            }
        }
    }
    
    private void Flip(){
        isFacingLeft = !isFacingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void StartRunning(){
        isRunning = true;
    }
    public void StopRunning(){
        isRunning = false;
    }

    public void Knockback(){
        Vector2 knockbackForce = knockback;
        knockbackForce.x *= -GetDirection(player.transform);
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    public int GetDirection(Transform playerTransform){
        if(transform.position.x > playerTransform.position.x)
            return -1;
        return 1;
    }
}

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
    private bool isFacingRight = true;
    private bool isRunning = false;
    
    public GameObject player;
    public float playerAwareness;
    private bool inRange = false;
    
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
        if(!inRange && isRunning){
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
        }else if(inRange && isRunning){
            if(player.transform.position.x < transform.position.x){
                if(!isFacingRight) Flip();
                transform.Translate(Vector2.left * Time.deltaTime * speed);
            }else {
                if(isFacingRight) Flip();
                transform.Translate(Vector2.right * Time.deltaTime * speed);
            }
        }
    }
    
    private void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }

    public void StartRunning(){
        isRunning = true;
    }
    public void StopRunning(){
        isRunning = false;
    }

    // void OnCollisionEnter2D(Collision2D collision){
    //     if (collision.collider.tag == "Player") {
    //         GetComponent<EnemyHealth>().TakeDamage(10);
    //     }
    // }
}

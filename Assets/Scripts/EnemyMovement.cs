using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject pointA;
    public GameObject pointB;
    private Animator animator;
    private Transform currentPoint;
    [SerializeField] private float speed = 4f;
    private bool isFacingRight = true;
    
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", true);
        currentPoint = pointA.transform;
    }
    public void FixedUpdate(){
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
    }
    
    private void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}

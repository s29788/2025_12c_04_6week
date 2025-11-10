using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D _rb;
    
    private float _xInput;
    [SerializeField] private float _speed;
    private bool _isFacingRight = true;
    
    [SerializeField] private float _jumpForce;
    private bool _performJump;
    private bool _isGrounded;
    
    [SerializeField] private float _coyoteTime = 0.2f;
    private float _coyoteTimeCounter;

    private bool _doubleJump;

    private void Awake(){
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Update(){
        _xInput = Input.GetAxisRaw("Horizontal");
        Flip();
        
        if(_isGrounded)
            _coyoteTimeCounter = _coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;
        
        if(Input.GetButtonDown("Jump") && (_coyoteTimeCounter > 0f || _doubleJump)){
            _performJump = true;
            _coyoteTimeCounter = 0f;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
            _doubleJump = !_doubleJump;
        }
    }
    private void FixedUpdate(){
        _rb.linearVelocity = new Vector2(_xInput*_speed, _rb.linearVelocity.y);

        if(_performJump){
            _performJump = false;
            _rb.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
        }
    }

    private void Flip(){
        if(_isFacingRight && _xInput < 0f || !_isFacingRight && _xInput > 0f){
            _isFacingRight = !_isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision){
        _isGrounded = true;
    }
    private void OnCollisionExit2D(Collision2D collision){
        _isGrounded = false;
    }
}

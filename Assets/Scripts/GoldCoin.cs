using TMPro;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Player"))
            animator.SetBool("isCollected", true);
    }

    public void DestroyCoin(){
        Destroy(gameObject);
    }

}

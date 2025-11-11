using TMPro;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private UIManager uiManager;   // <- przypnij ręcznie obiekt GameManager ze sceny
        
    void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isCollected", true);
            uiManager.coinText.GetComponent<Animator>().SetTrigger("coinCollected");
        }
    }
    
    public void DestroyCoin(){
        Destroy(gameObject);
    }
}
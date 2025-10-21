using UnityEngine;

public class GoldCoinCollect : MonoBehaviour {
    public int goldCoinCount;
    
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Coin"))
            goldCoinCount++;
    }
}
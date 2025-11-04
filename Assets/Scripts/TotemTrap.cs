using UnityEngine;

public class TotemTrap : MonoBehaviour
{ 
    [HideInInspector] public Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isAttacking", false);
    }
}

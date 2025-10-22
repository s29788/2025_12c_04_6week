using UnityEngine;

public class TotemTrap : MonoBehaviour
{ 
    [HideInInspector] public Animator _anim;

    public Animator GetAnimator()
    {
        return _anim;
    }
    
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.SetBool("isAttacking", false);
    }
}

using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _cooldown;
    [SerializeField] private string _attackAnimationStateName = "Attack"; 
    [SerializeField] private float _transitionDuration = 0.1f;
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponentInParent<TotemTrap>().GetAnimator();
        if (_anim == null)
        {
            Debug.LogError("Animator component not found! Make sure it is attached to the parent object.");
        }
    }
    
    private void Start()
    {
        if (_anim != null)
        {
            StartCoroutine(SpawnCoroutine());
        }
    }
    
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            _anim.SetBool("isAttacking", true);
            yield return new WaitForSeconds(_transitionDuration); 
            
            AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
            float animationClipLength = 0f;
            
            if (stateInfo.IsName(_attackAnimationStateName))
                animationClipLength = stateInfo.length;
            
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
            
            yield return new WaitForSeconds(animationClipLength); 
            
            _anim.SetBool("isAttacking", false);
            
            float remainingCooldown = _cooldown - animationClipLength - _transitionDuration;
            
            if (remainingCooldown > 0)
                yield return new WaitForSeconds(remainingCooldown);
        }
    }
}
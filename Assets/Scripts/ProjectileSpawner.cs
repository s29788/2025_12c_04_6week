using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _cooldown;
    [SerializeField] private string _attackAnimationStateName = "Attack"; 
    [SerializeField] private float _transitionDuration = 0.1f;
    private TotemTrap _totemTrap;

    private void Awake()
    {
        _totemTrap = GetComponentInParent<TotemTrap>();
    }
    
    private void Start()
    {
        if (_totemTrap.anim != null)
        {
            StartCoroutine(SpawnCoroutine());
        }
    }
    
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            _totemTrap.anim.SetTrigger("Attack");
            yield return new WaitForSeconds(_transitionDuration); 
            
            AnimatorStateInfo stateInfo = _totemTrap.anim.GetCurrentAnimatorStateInfo(0);
            float animationClipLength = 0f;
            
            if (stateInfo.IsName(_attackAnimationStateName))
                animationClipLength = stateInfo.length;
            
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
            
            yield return new WaitForSeconds(animationClipLength); 
            
            float remainingCooldown = _cooldown - animationClipLength - _transitionDuration;
            
            if (remainingCooldown > 0)
                yield return new WaitForSeconds(remainingCooldown);
        }
    }
}
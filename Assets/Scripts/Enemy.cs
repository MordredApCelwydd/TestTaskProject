using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles enemy movement, targeting, line of sight detection, and shooting behavior.
/// Implements IMove and IAttack interfaces to communicate animation state.
/// </summary>
public class Enemy : MonoBehaviour, IMove, IAttack
{
    private NavMeshAgent _navMeshAgent;

    private GameObject _target;

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float firingCooldown;

    private ObjectPool<Projectile> _projectilePool;
    
    private bool _hasLineOfSight;
    
    private Coroutine _shootingCoroutine;
    
    public event Action<bool> IsMoving;
    public event Action<bool> IsAttacking;
    
    private void OnEnable()
    {
        _shootingCoroutine = StartCoroutine(Shooter());
    }

    private void OnDisable()
    {
        StopCoroutine(_shootingCoroutine);
    }

    private void Start()
    {
        _hasLineOfSight = false;
        _target = PlayerSingleton.Instance;

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        if(Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), 100, playerLayerMask.value))
        {
            _hasLineOfSight = true;
        }
        else
        {
            _hasLineOfSight = false;
        }
        
        _navMeshAgent.SetDestination(_target.transform.position);
            
        IsMoving?.Invoke(_navMeshAgent.velocity.sqrMagnitude > 0.01f);
    }

    
    /// <summary>
    /// Continuously checks for line of sight and shoots projectiles if true.
    /// Triggers and resets the attacking animation state.
    /// </summary>
    private IEnumerator Shooter()
    {
        while (true)
        {
            if(_hasLineOfSight)
            {
                IsAttacking?.Invoke(true);
                Projectile instance = _projectilePool.GetElement(transform.position + transform.forward);
                instance.transform.position += Vector3.up;
                instance.transform.rotation = transform.rotation;
                yield return new WaitForSeconds(firingCooldown/2);
                IsAttacking?.Invoke(false);
                yield return new WaitForSeconds(firingCooldown/4);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public void SetProjectilePool(ObjectPool<Projectile> pool)
    {
        _projectilePool = pool;
    }
}



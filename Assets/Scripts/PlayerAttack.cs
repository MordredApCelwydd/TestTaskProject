using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player attack logic, detecting and killing the nearest enemy within range.
/// </summary>
public class PlayerAttack : MonoBehaviour, IAttack
{
    [SerializeField] private InputActionReference attackAction;
    
    [SerializeField] private LayerMask enemy;
    [SerializeField] private int killRange;
    
    public event Action<bool> IsAttacking;

    private void Start()
    {
        attackAction.action.Enable();
    }

    private void OnEnable()
    {
        attackAction.action.started += OnKillEnemyPressed;
    }

    private void OnDisable()
    {
        attackAction.action.started -= OnKillEnemyPressed;
    }

    /// <summary>
    /// Called when the kill enemy input action is performed.
    /// Detects the nearest enemy within killRange and kills it.
    /// </summary>
    private void OnKillEnemyPressed(InputAction.CallbackContext context)
    {
        IsAttacking?.Invoke(true);
        float distance;
        Collider target = null;
        float nearestDistance = float.MaxValue;
        
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, killRange, enemy);
        foreach (Collider hit in hits)
        {
            distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                target = hit;
            }
        }
        
        if (target)
        {
            target.GetComponent<Killable>().Die();
        }

        StartCoroutine(ResetAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        yield return null; 
        IsAttacking?.Invoke(false);
    }
}

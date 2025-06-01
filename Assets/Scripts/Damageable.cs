using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles receiving damage and notifies listeners using the ITakeDamage interface.
/// </summary>
public class Damageable : MonoBehaviour, ITakeDamage
{
    public void TakeDamage()
    {
        IsTakingDamage?.Invoke(true);
        StartCoroutine(ResetTakeDamageFlag());
    }

    private IEnumerator ResetTakeDamageFlag()
    {
        yield return null;
        IsTakingDamage?.Invoke(false);
    }
    
    public event Action<bool> IsTakingDamage;
}

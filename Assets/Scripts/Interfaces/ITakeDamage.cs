using System;
using UnityEngine;

/// <summary>
/// Interface for objects that can take damage.
/// Notifies listeners about the taking damage state
/// </summary>
public interface ITakeDamage
{
    public event Action<bool> IsTakingDamage;
}

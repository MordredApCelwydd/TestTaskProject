using System;
using UnityEngine;

/// <summary>
/// Interface for objects that can attack.
/// Notifies listeners about the attack state
/// </summary>
public interface IAttack
{
    public event Action<bool> IsAttacking;
}

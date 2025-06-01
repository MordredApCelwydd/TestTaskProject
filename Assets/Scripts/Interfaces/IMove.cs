using System;
using UnityEngine;

/// <summary>
/// Interface for objects that can move.
/// Notifies listeners about the movement state
/// </summary>
public interface IMove
{
    public event Action<bool> IsMoving;
}

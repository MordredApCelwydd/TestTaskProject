using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class for objects that use object pools, handles returning them to the pool.
/// </summary>
public class Killable : MonoBehaviour
{
    public void Die()
    {
        gameObject.SetActive(false);
    }
}

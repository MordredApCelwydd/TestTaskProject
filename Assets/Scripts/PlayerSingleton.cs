using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Simple singleton for the player class, considering that the game is singleplayer
/// </summary>
public class PlayerSingleton : MonoBehaviour
{
    public static GameObject Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this.GameObject())
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = gameObject;
        }
    }
}

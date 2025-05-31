using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

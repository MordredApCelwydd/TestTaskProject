using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private bool isPoolExpandable;
    [SerializeField] private int poolDefaultSize;

    [SerializeField] private InputActionReference spawnAction;
    
    private GameObject _player;
    
    private ObjectPool<MonoBehaviour> _enemyPool;
    private ObjectPool<MonoBehaviour> _projectilePool;
    
    private void Start()
    {
        spawnAction.action.Enable();
        _player = PlayerSingleton.Instance;
        _enemyPool = new ObjectPool<MonoBehaviour>(enemy.GetComponent<Spawnable>(), isPoolExpandable, poolDefaultSize);
    }

    private void OnEnable()
    {
        spawnAction.action.started += OnSpawnPressed;
    }

    private void OnDisable()
    {
        spawnAction.action.started -= OnSpawnPressed;
    }

    private void OnSpawnPressed(InputAction.CallbackContext context)
    {
        Vector3 spawnPoint = GetSpawnPoint(5, 10, 10);
        if (spawnPoint != Vector3.negativeInfinity)
        {
            _enemyPool.GetElement(spawnPoint);
        }
    }

    private Vector3 GetSpawnPoint(float minRange, float maxRange, int spawnAttempts)
    {
        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 point = new Vector2(_player.transform.position.x, _player.transform.position.z) + Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
            Vector3 spawnPoint = new Vector3(point.x, _player.transform.position.y, point.y);

            if (ValidateSpawnPoint(spawnPoint))
            {
                return spawnPoint;
            }
        }

        return Vector3.negativeInfinity;
    }

    private bool ValidateSpawnPoint(Vector3 spawnPoint)
    {
        if (NavMesh.SamplePosition(spawnPoint, out _, _player.transform.position.y + 0.1f, NavMesh.AllAreas))
        {
            CapsuleCollider capsule = enemy.GetComponent<CapsuleCollider>();
            
            Vector3 bottom = spawnPoint + Vector3.up * capsule.radius;
            Vector3 top = spawnPoint + Vector3.up * (capsule.height - capsule.radius);

            if (!Physics.CheckCapsule(bottom, top, capsule.radius))
            {
                return true;
            }
        }
        return false;
    }
}

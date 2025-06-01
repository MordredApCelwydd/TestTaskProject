using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Handles spawning enemies around player, as well as creating object pools for enemies and their projectiles
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;
    [SerializeField] private int maxSpawnAttempts;
    
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject projectile;
    
    [SerializeField] private bool arePoolsExpandable;
    [SerializeField] private int poolDefaultSize;
    
    [SerializeField] private Button spawnButton;
    
    private GameObject _player;
    
    private ObjectPool<Enemy> _enemyPool;
    private ObjectPool<Projectile> _projectilePool;
    
    private void Start()
    {
        _player = PlayerSingleton.Instance;
        _enemyPool = new ObjectPool<Enemy>(enemy.GetComponent<Enemy>(), arePoolsExpandable, poolDefaultSize);
        _projectilePool = new ObjectPool<Projectile>(projectile.GetComponent<Projectile>(), arePoolsExpandable, poolDefaultSize);
    }

    /// <summary>
    /// Called whenever the spawn button is pressed. Attempts to find a valid spawnpoint for an enemy,
    /// then spawns an enemy from an ObjectPool there
    /// </summary>
    public void OnSpawnPressed()
    {
        StartCoroutine(GetSpawnPointCoroutine(minSpawnDistance, maxSpawnDistance, maxSpawnAttempts, spawnPoint =>
        {
            if (spawnPoint != Vector3.zero)
            {
                Enemy instance = _enemyPool.GetElement(spawnPoint);
                instance.SetProjectilePool(_projectilePool);
            }
        }));
    }
    
    /// <summary>
    /// Coroutine for finding a valid spawnpoint for the enemy around the player.
    /// Calls the callback with the valid position or Vector3.zero if no valid spawn point was found.
    /// </summary>
    /// <param name="minRange">Minimum distance from the player to the spawn point</param>
    /// <param name="maxRange">Maximum distance from the player to the spawn point</param>
    /// <param name="spawnAttempts">Maximum attempts to find a valid spawn point</param>
    /// <param name="onSpawnPointFound">Callback invoked with a valid spawn point or Vector3.zero</param>
    private IEnumerator GetSpawnPointCoroutine(float minRange, float maxRange, int spawnAttempts, Action<Vector3> onSpawnPointFound)
    {
        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 point = new Vector2(_player.transform.position.x, _player.transform.position.z) 
                            + Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
            Vector3 spawnPoint = new Vector3(point.x, _player.transform.position.y, point.y);

            if (ValidateSpawnPoint(spawnPoint))
            {
                onSpawnPointFound?.Invoke(spawnPoint);
                yield break; 
            }

            yield return null; 
        }
        onSpawnPointFound?.Invoke(Vector3.zero); 
    }
    
    /// <summary>
    /// Validates whether the given point is suitable for spawning an enemy.
    /// The point must be on a navmesh and there must be no collisions with other objects.
    /// </summary>
    /// <param name="spawnPoint">Spawn point to be validated</param>
    /// <returns>True if the point is valid, false otherwise</returns>
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

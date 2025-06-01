using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles projectile logic, including its speed and lifespan. Meant to be used with an object pool, therefore
/// the projectile won't destroyed at the end of its lifespan.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private float lifespan;

    private void OnEnable()
    {
        StartCoroutine(LifeSpanCounter());
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerSingleton.Instance.gameObject)
        {
            PlayerSingleton.Instance.GetComponent<Damageable>()?.TakeDamage();
        }

        gameObject.SetActive(false);
    }
    
    private IEnumerator LifeSpanCounter()
    {
        yield return new WaitForSeconds(lifespan);
        gameObject.SetActive(false);
    }
}

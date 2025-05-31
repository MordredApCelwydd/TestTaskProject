using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    private GameObject _target;

    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private float firingCooldown;

    [SerializeField] private GameObject projectile;
    
    private bool _hasLineOfSight;

    private Coroutine _shootingCoroutine;
    
    private void Start()
    {
        _hasLineOfSight = false;
        _target = PlayerSingleton.Instance;

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _shootingCoroutine = StartCoroutine(Shooter());
    }
    
    private void Update()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 100, playerLayerMask.value))
        {
            _hasLineOfSight = true;
        }
        else
        {
            _hasLineOfSight = false;
        }
        
        _navMeshAgent.SetDestination(_target.transform.position);
    }

    private IEnumerator Shooter()
    {
        while (true)
        {
            if(_hasLineOfSight)
            {
                Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            }
            yield return new WaitForSeconds(firingCooldown);
        }
    }
}



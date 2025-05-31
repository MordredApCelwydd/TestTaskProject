using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;

    [SerializeField] private float moveAcceleration;
    [SerializeField] private float maxSpeed;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private LayerMask enemy;
    [SerializeField] private int killRange;
    
    private Rigidbody _rb;
    private Vector3 _forceDirection = Vector3.zero;

    private void OnEnable()
    {
        attackAction.action.started += OnKillEnemyPressed;
    }

    private void OnDisable()
    {
        attackAction.action.started -= OnKillEnemyPressed;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        moveAction.action.Enable();
        attackAction.action.Enable();
    }

    private void FixedUpdate()
    {
        _forceDirection += moveAction.action.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * moveAcceleration; 
        _forceDirection += moveAction.action.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * moveAcceleration; 
        
        _rb.AddForce(_forceDirection, ForceMode.Impulse);
        _forceDirection = Vector3.zero;

        Vector3 horizontalVelocity = _rb.linearVelocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
           _rb.linearVelocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * _rb.linearVelocity.y;
        }
    }

    private Vector3 GetCameraRight(Camera cam)
    {
        Vector3 right = cam.transform.right;
        right.y = 0;
        return right.normalized;
    }
    
    private Vector3 GetCameraForward(Camera cam)
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private void OnKillEnemyPressed(InputAction.CallbackContext context)
    {
        float distance = 0;
        Collider target = null;
        float nearestDistance = float.MaxValue;
        
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, killRange, enemy);
        foreach (Collider hit in hits)
        {
            distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                target = hit;
            }
        }
        
        if (target)
        {
            target.GetComponent<Killable>().Die();
        }
    }
}

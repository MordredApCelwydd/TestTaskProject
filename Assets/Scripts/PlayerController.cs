using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement based on input actions, applying physics forces,
/// and rotates the player to face movement direction.
/// Implements the IMove interface to notify about movement state changes
/// </summary>
public class PlayerController : MonoBehaviour, IMove
{
    [SerializeField] private InputActionReference moveAction;

    [SerializeField] private float moveAcceleration;
    [SerializeField] private float maxSpeed;

    [SerializeField] private Camera playerCamera;
    
    private Rigidbody _rb;
    private Vector3 _forceDirection = Vector3.zero;

    public event Action<bool> IsMoving;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        moveAction.action.Enable();
    }
    
    
    /// <summary>
    /// Applies movement forces each physics update based on player input.
    /// Limits the speed, rotates player to face movement direction,
    /// and triggers IsMoving event.
    /// </summary>
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

        LookAhead();
        
        IsMoving?.Invoke(horizontalVelocity.sqrMagnitude > 0.01f);
    }

    /// <summary>
    /// Gets the right vector of the camera projected onto the horizontal plane.
    /// </summary>
    /// <param name="cam">Main camera.</param>
    /// <returns>Normalized horizontal right vector of the camera.</returns>
    private Vector3 GetCameraRight(Camera cam)
    {
        Vector3 right = cam.transform.right;
        right.y = 0;
        return right.normalized;
    }
    
    /// <summary>
    /// Gets the forward vector of the camera projected onto the horizontal plane.
    /// </summary>
    /// <param name="cam">Main camera.</param>
    /// <returns>Normalized horizontal forward vector of the camera.</returns>
    private Vector3 GetCameraForward(Camera cam)
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    /// <summary>
    /// Rotates the player to face the direction of movement,
    /// or stops rotation if there is no input or velocity.
    /// </summary>
    private void LookAhead()
    {
        Vector3 direction = _rb.linearVelocity;
        direction.y = 0;

        if (moveAction.action.ReadValue<Vector2>().sqrMagnitude > 0.01f && direction.sqrMagnitude > 0.1f)
        {
            _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }
}

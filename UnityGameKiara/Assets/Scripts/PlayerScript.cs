using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerScript : MonoBehaviour
{
    //TODO: Coyote time?
    //TODO: Dash ability

    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;

    [Header("Player actions")]
    [SerializeField]
    private InputActionReference _movementAction;
    [SerializeField]
    private InputActionReference _jumpAction;

    [Header("Player stats")]
    [SerializeField]
    private float _movementSpeed = 15.0f;
    [SerializeField]
    private float _jumpPower = 24.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _boxCollider = GetComponent<BoxCollider2D>();

        _jumpAction.action.started += OnJumpActionStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        _rigidBody.linearVelocityX = _movementAction.action.ReadValue<float>() * _movementSpeed;
    }

    // Are we on the ground?
    private bool IsGrounded()
    {
        List<RaycastHit2D> results = new();

        int worldLayerMask = LayerMask.NameToLayer("World");

        ContactFilter2D filter = new();
        filter.layerMask = new();
        filter.layerMask.value = worldLayerMask;
        filter.useLayerMask = true;

        return _boxCollider.Cast(Vector2.down, results, 0.1f, true) >= 1;
    }

    private void Jump()
    {
        _rigidBody.linearVelocityY = _jumpPower;
    }

    private void OnJumpActionStarted(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
            Jump();
    }
}

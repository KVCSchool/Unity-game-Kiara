using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerScript : MonoBehaviour
{
    //TODO: Coyote time?
    //TODO: Dash bounces

    private bool _dashing = false;
    private int _lastMovementDirection = 0;

    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;

    [Header("Player actions")]
    [SerializeField]
    private InputActionReference _movementAction;
    [SerializeField]
    private InputActionReference _jumpAction;
    [SerializeField]
    private InputActionReference _dashAction;

    [Header("Player stats")]
    [SerializeField]
    private float _movementSpeed = 15.0f;
    [SerializeField]
    private float _jumpPower = 24.0f;
    [SerializeField]
    private float _dashSpeed = 64.0f;

    public bool Dashing { get => _dashing; }

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float JumpPower { get => _jumpPower; set => _jumpPower = value; }
    public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _boxCollider = GetComponent<BoxCollider2D>();

        _jumpAction.action.started += OnJumpActionStarted;
        _dashAction.action.started += OnDashActionStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        float movementDirection = _movementAction.action.ReadValue<float>();

        if (!_dashing)
            _rigidBody.linearVelocityX = movementDirection * _movementSpeed;

        if (movementDirection > 0.0f)
            _lastMovementDirection = 1;
        else if (movementDirection < 0.0f)
            _lastMovementDirection = -1;
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

    // Dash in last moved direction
    private void Dash()
    {
        _dashing = true;

        if (_lastMovementDirection == 0)
            _lastMovementDirection = 1;

        //down-left or down-right depending on last direction moved
        float dashAngle = (_lastMovementDirection == 1) ? -Mathf.PI / 4.0f : -Mathf.PI / 4.0f * 3.0f;

        Vector2 dashDirection = new(Mathf.Cos(dashAngle), Mathf.Sin(dashAngle));

        _rigidBody.linearVelocity = dashDirection * _dashSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: ground splat effect
        if (_dashing)
        {
            _dashing = false;
            Debug.Log("Player Splat!");
        }
    }

    private void OnJumpActionStarted(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
            Jump();
    }

    private void OnDashActionStarted(InputAction.CallbackContext obj)
    {
        if (!IsGrounded() && !_dashing)
            Dash();
    }
}

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D)]
public class Player : MonoBehaviour
{
    //TODO: Coyote time?

    private bool _dashing = false;
    private int _lastMovementDirection = 0;

    private Rigidbody2D _rigidBody;

    [SerializeField]
    private BoxCollider2D _groundCheck;

    private BoxCollider2D _boxCollider;
    private CircleCollider2D _circleCollider;

    [Header("Player actions")]
    [SerializeField]
    private InputActionReference _movementAction;
    [SerializeField]
    private InputActionReference _jumpAction;
    [SerializeField]
    private InputActionReference _dashAction;

    [Header("Physics Materials")]
    [SerializeField]
    private PhysicsMaterial2D _standardPhysics;
    [SerializeField]
    private PhysicsMaterial2D _dashPhysics;

    [Header("Player stats")]
    [SerializeField]
    private float _movementSpeed = 15.0f;
    [SerializeField]
    private float _jumpPower = 24.0f;
    [SerializeField]
    private float _dashSpeed = 64.0f;
    [SerializeField]
    private float _acceleration = 120.0f;
    [SerializeField]
    private float _friction = 60.0f;

    public bool Dashing { get => _dashing; }

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float JumpPower { get => _jumpPower; set => _jumpPower = value; }
    public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }
    public float Acceleration { get => _acceleration; set => _acceleration = value; }
    public float Friction { get => _friction; set => _friction = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.sharedMaterial = _standardPhysics;

        _boxCollider = GetComponent<BoxCollider2D>();
        _circleCollider = GetComponent<CircleCollider2D>();

        _jumpAction.action.started += OnJumpActionStarted;
        _dashAction.action.started += OnDashActionStarted;

        UpdateActiveHitbox();
    }

    // Can the player accelerate into the given direction with the specified strength?
    // Player can (de)accelerate while not dashing until it reaches its movement speed multiplied by strength.
    // Direction is the sign of the movement action axis.
    private bool CanAccelerateMovement(int direction, float strength)
    {
        if (_dashing)
            return false;

        if (direction == 0 || strength == 0.0f)
            return false;

        //max movement velocity already reached
        if (direction == 1 && _rigidBody.linearVelocityX >= _movementSpeed * strength)
            return false;

        //min movement velocity already reached
        if (direction == -1 && _rigidBody.linearVelocityX <= _movementSpeed * -strength)
            return false;

        return true;
    }

    // Update is called once per frame
    private void Update()
    {
        //Update movement velocity

        float movementAxis = _movementAction.action.ReadValue<float>();

        //Important: sign returns 1 when movementAxis is 0.
        int movementDirection = (int)Mathf.Sign(movementAxis);
        float movementStrength = Mathf.Abs(movementAxis);

        if (CanAccelerateMovement(movementDirection, movementStrength))
            _rigidBody.linearVelocityX = Mathf.Clamp(_rigidBody.linearVelocityX + _acceleration * movementDirection * Time.deltaTime, -_movementSpeed * movementStrength, _movementSpeed * movementStrength);
        //Horizontal friction, Unity's one causes the player to get stuck on walls :/
        else if (_rigidBody.linearVelocityX != 0.0f)
            _rigidBody.linearVelocityX = Mathf.Sign(_rigidBody.linearVelocityX) * Mathf.Max(Mathf.Abs(_rigidBody.linearVelocityX) - _friction * Time.deltaTime, 0.0f);
        
        if (movementStrength != 0.0f)
            _lastMovementDirection = movementDirection;
    }

    // Are we on the ground?
    private bool IsGrounded()
    {
        return _groundCheck.IsTouchingLayers(_groundCheck.includeLayers);
    }

    private void Jump()
    {
        _rigidBody.linearVelocityY = _jumpPower;
    }

    private void UpdateActiveHitbox()
    {
        _boxCollider.enabled = !_dashing;
        _circleCollider.enabled = _dashing;
    }

    // Dash in last moved direction
    private void Dash()
    {
        _dashing = true;

        UpdateActiveHitbox();

        if (_lastMovementDirection == 0)
            _lastMovementDirection = 1;

        //down-left or down-right depending on last direction moved
        float dashAngle = (_lastMovementDirection == 1) ? -Mathf.PI / 4.0f : -Mathf.PI / 4.0f * 3.0f;

        Vector2 dashDirection = new(Mathf.Cos(dashAngle), Mathf.Sin(dashAngle));

        _rigidBody.linearVelocity = dashDirection * _dashSpeed;
        _rigidBody.sharedMaterial = _dashPhysics;
    }

    // Collided with something while dashing.
    private void OnDashCollision(Collision2D collision)
    {
        _dashing = false;

        UpdateActiveHitbox();

        _rigidBody.linearVelocityX *= 1.2f;
        _rigidBody.sharedMaterial = _standardPhysics;

        //TODO: ground splat effect
        Debug.Log("Player Splat!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dashing)
            OnDashCollision(collision);
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

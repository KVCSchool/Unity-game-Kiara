using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator), typeof(GroundedMovement))]
public class PlayerController : MonoBehaviour
{
    //TODO: Coyote time?
    //TODO: this is starting to do a lot of things at once, might be time to separate this into multiple behaviours.

    [SerializeField]
    private bool _controllable = true;

    private int _lastMovementDirection = 0;

    private bool _canDash = true;

    private bool _grounded = false;
    private bool _dashing = false;

    private float _dashCooldown = 0.0f;

    private Rigidbody2D _rigidBody;
    private Animator _animator;

    [SerializeField]
    private BoxCollider2D _groundCheck;

    private BoxCollider2D _boxCollider;
    private CircleCollider2D _circleCollider;

    private GroundedMovement _movement;

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

    [Header("Player movement")]
    [SerializeField]
    private float _standardMovementSpeed = 15.0f;
    [SerializeField]
    private float _dashMovementSpeed = 5.0f;
    [SerializeField]
    private float _dashStrength = 64.0f;
    [SerializeField]
    private float _standardFriction = 60.0f;
    [SerializeField]
    private float _dashFriction = 15.0f;
    [SerializeField]
    private float _dashCooldownDurationSeconds = 0.25f;

    public bool Controllable { get => _controllable; set => _controllable = value; }
    public bool Dashing { get => _dashing; }

    public float StandardMovementSpeed { get => _standardMovementSpeed; set => _standardMovementSpeed = value; }
    public float DashMovementSpeed { get => _dashMovementSpeed; set => _dashMovementSpeed = value; }

    public float DashStrength { get => _dashStrength; set => _dashStrength = value; }
    public float StandardFriction { get => _standardFriction; set => _standardFriction = value; }
    public float DashFriction { get => _dashFriction; set => _dashFriction = value; }
    public float DashCooldownDurationSeconds { get => _dashFriction; set => _dashFriction = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.sharedMaterial = _standardPhysics;

        _movement = GetComponent<GroundedMovement>();

        _animator = GetComponent<Animator>();

        _boxCollider = GetComponent<BoxCollider2D>();
        _circleCollider = GetComponent<CircleCollider2D>();

        _jumpAction.action.started += OnJumpActionStarted;

        _dashAction.action.started += OnDashActionStarted;
        _dashAction.action.canceled += OnDashActionCancelled;

        UpdateActiveHitbox();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_dashCooldown > 0.0f)
            _dashCooldown -= Time.deltaTime;

        //automatically exit dash form if too slow
        if (_dashing && Mathf.Abs(_movement.Velocity.magnitude) < 0.5f)
            DashEnd();

        //Update movement velocity

        float movementAxis = _movementAction.action.ReadValue<float>();

        //Important: sign returns 1 when movementAxis is 0.
        int movementDirection = (movementAxis != 0.0f) ? (int)Mathf.Sign(movementAxis) : 0;

        _movement.MoveIn(new(movementDirection, 0.0f));

        if (movementDirection != 0.0f)
            _lastMovementDirection = movementDirection;
    }

    public void Jump()
    {
        if (!_controllable || !_grounded || _dashing)
            return;

        _movement.Jump();
    }

    private void UpdateActiveHitbox()
    {
        _boxCollider.enabled = !_dashing;
        _circleCollider.enabled = _dashing;
    }

    // Dash in last moved direction
    public void Dash()
    {
        if (!_controllable || _dashing || !_canDash || _dashCooldown > 0.0f)
            return;

        _dashing = true;
        _canDash = _grounded;
        _dashCooldown = _dashCooldownDurationSeconds;

        _movement.Speed = _dashMovementSpeed;
        _movement.Friction = _dashFriction;

        UpdateActiveHitbox();

        //dash horizontally or down diagonally (to the right)
        float dashAngle = (_grounded) ? 0.0f : -Mathf.PI / 4.0f;

        //if moving left, flip angle horizontally (to the left)
        if (_lastMovementDirection == -1)
            dashAngle = Mathf.PI - dashAngle;

        Vector2 dashDirection = new(Mathf.Cos(dashAngle), Mathf.Sin(dashAngle));

        _movement.Velocity = dashDirection * _dashStrength;
        _rigidBody.sharedMaterial = _dashPhysics;

        _animator.SetBool("Dashing", true);
    }

    public void DashEnd()
    {
        if (!_dashing)
            return;

        _dashing = false;
        _movement.Speed = _standardMovementSpeed;
        _movement.Friction = _standardFriction;

        UpdateActiveHitbox();

        _rigidBody.sharedMaterial = _standardPhysics;

        _animator.SetBool("Dashing", false);

        Debug.Log("Dash ended!");
    }

    public void OnBecameGrounded()
    {
        Debug.Log("Player became grounded!");

        _grounded = true;
        _canDash = true;
        _movement.CanJump = true;
    }

    public void OnBecameAirbone()
    {
        Debug.Log("Player became airbone!");

        _grounded = false;
        _movement.CanJump = false;
    }

    // Collided with something while dashing.
    private void OnDashCollision(Collision2D collision)
    {
        _canDash = true;

        //player attack
        if (collision.gameObject.TryGetComponent(out DamageHitbox hitbox))
            hitbox.Damage(1);

        //TODO: ground splat effect
        Debug.Log("Player Splat!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dashing)
            OnDashCollision(collision);
    }

    private void OnJumpActionStarted(InputAction.CallbackContext obj)
        => Jump();

    private void OnDashActionStarted(InputAction.CallbackContext obj)
        => Dash();

    private void OnDashActionCancelled(InputAction.CallbackContext obj)
        => DashEnd();
}

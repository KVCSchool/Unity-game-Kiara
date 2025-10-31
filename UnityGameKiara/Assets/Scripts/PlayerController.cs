using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
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
    private float _movementSpeed = 15.0f;
    [SerializeField]
    private float _standardMovementSpeed = 15.0f;
    [SerializeField]
    private float _dashMovementSpeed = 5.0f;
    [SerializeField]
    private float _jumpPower = 24.0f;
    [SerializeField]
    private float _dashStrength = 64.0f;
    [SerializeField]
    private float _acceleration = 120.0f;
    [SerializeField]
    private float _standardFriction = 60.0f;
    [SerializeField]
    private float _dashFriction = 15.0f;
    [SerializeField]
    private float _dashCooldownDurationSeconds = 0.25f;

    public bool Controllable { get => _controllable; set => _controllable = value; }
    public bool Dashing { get => _dashing; }

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float StandardMovementSpeed { get => _standardMovementSpeed; set => _standardMovementSpeed = value; }
    public float DashMovementSpeed { get => _dashMovementSpeed; set => _dashMovementSpeed = value; }

    public float JumpPower { get => _jumpPower; set => _jumpPower = value; }
    public float DashStrength { get => _dashStrength; set => _dashStrength = value; }
    public float Acceleration { get => _acceleration; set => _acceleration = value; }
    public float StandardFriction { get => _standardFriction; set => _standardFriction = value; }
    public float DashFriction { get => _dashFriction; set => _dashFriction = value; }
    public float DashCooldownDurationSeconds { get => _dashFriction; set => _dashFriction = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.sharedMaterial = _standardPhysics;

        _animator = GetComponent<Animator>();

        _boxCollider = GetComponent<BoxCollider2D>();
        _circleCollider = GetComponent<CircleCollider2D>();

        _jumpAction.action.started += OnJumpActionStarted;

        _dashAction.action.started += OnDashActionStarted;
        _dashAction.action.canceled += OnDashActionCancelled;

        UpdateActiveHitbox();
    }

    // Can the player accelerate into the given direction with the specified strength?
    // Player can (de)accelerate while not dashing until it reaches its movement speed multiplied by strength.
    // Direction is the sign of the movement action axis.
    private bool CanAccelerateMovement(int direction, float strength)
    {
        if (!_controllable)
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
        if (_dashCooldown > 0.0f)
            _dashCooldown -= Time.deltaTime;

        //Update movement velocity

        float movementAxis = _movementAction.action.ReadValue<float>();

        //Important: sign returns 1 when movementAxis is 0.
        int movementDirection = (int)Mathf.Sign(movementAxis);
        float movementStrength = Mathf.Abs(movementAxis);

        //automatically exit dash form if too slow
        if (_dashing && Mathf.Abs(_rigidBody.linearVelocity.magnitude) < 0.5f )
            DashEnd();

        if (CanAccelerateMovement(movementDirection, movementStrength))
        {
            _rigidBody.linearVelocityX = Mathf.Clamp(_rigidBody.linearVelocityX + _acceleration * movementDirection * Time.deltaTime, -_movementSpeed * movementStrength, _movementSpeed * movementStrength);
        }
        //Horizontal friction, Unity's one causes the player to get stuck on walls :/
        else if (_rigidBody.linearVelocityX != 0.0f)
        {
            float friction = (_dashing) ? _dashFriction : _standardFriction;
            _rigidBody.linearVelocityX = Mathf.Sign(_rigidBody.linearVelocityX) * Mathf.Max(Mathf.Abs(_rigidBody.linearVelocityX) - friction * Time.deltaTime, 0.0f);
        }
        
        if (movementStrength != 0.0f)
            _lastMovementDirection = movementDirection;
    }

    public void Jump()
    {
        if (!_controllable || !_grounded || _dashing)
            return;

        _rigidBody.linearVelocityY = _jumpPower;
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

        _movementSpeed = _dashMovementSpeed;

        UpdateActiveHitbox();

        //dash horizontally or down diagonally (to the right)
        float dashAngle = (_grounded) ? 0.0f : -Mathf.PI / 4.0f;

        //if moving left, flip angle horizontally (to the left)
        if (_lastMovementDirection == -1)
            dashAngle = Mathf.PI - dashAngle;

        Vector2 dashDirection = new(Mathf.Cos(dashAngle), Mathf.Sin(dashAngle));

        _rigidBody.linearVelocity = dashDirection * _dashStrength;
        _rigidBody.sharedMaterial = _dashPhysics;

        _animator.SetBool("Dashing", true);
    }

    public void DashEnd()
    {
        if (!_dashing)
            return;

        _dashing = false;
        _dashCooldown = _dashCooldownDurationSeconds;
        _movementSpeed = _standardMovementSpeed;

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
    }

    public void OnBecameAirbone()
    {
        Debug.Log("Player became airbone!");

        _grounded = false;
    }

    // Collided with something while dashing.
    private void OnDashCollision(Collision2D collision)
    {
        _canDash = true;

        //TODO: player attack
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

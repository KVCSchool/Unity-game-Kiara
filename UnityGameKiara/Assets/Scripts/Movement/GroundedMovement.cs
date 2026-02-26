using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundedMovement : Movement
{
    private Rigidbody2D _rigidBody;

    [Header("Jumping")]
    [SerializeField]
    private bool _canJump = true;
    [SerializeField]
    private float _jumpPower = 24.0f;

    private Vector2 _direction;

    public override Vector2 Position
    {
        get => _rigidBody.position;
        set => _rigidBody.position = value;
    }

    public override Vector2 Velocity
    {
        get => _rigidBody.linearVelocity;
        set => _rigidBody.linearVelocity = value;
    }

    public bool CanJump { get => _canJump; set => _canJump = value; }
    public float JumpPower { get => _jumpPower; set => _jumpPower = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Sign(num), but returns 0 if 0 passed.
    private float SignZero(float f)
        => (f != 0.0f) ? Mathf.Sign(f) : 0.0f;

    private void Update()
    {
        //Important: sign returns 1 when direction x is 0.
        int direction = (int)SignZero(_direction.x);

        if (CanAccelerate())
            VelocityX = Mathf.Clamp(VelocityX + Acceleration * direction * Time.deltaTime, -Speed, Speed);
        //Horizontal friction, Unity's one causes the player to get stuck on walls :/
        else if (VelocityX != 0.0f)
            VelocityX = Mathf.Sign(VelocityX) * Mathf.Max(Mathf.Abs(VelocityX) - Friction * Time.deltaTime, 0.0f);
    }

    // Can we accelerate into the current direction?
    protected override bool CanAccelerate()
    {
        if (!CanMove)
            return false;

        //Important: sign returns 1 when movementAxis is 0.
        int direction = (int)SignZero(_direction.x);

        if (direction == 0)
            return false;

        //max movement velocity already reached
        if (direction == 1 && VelocityX >= Speed)
            return false;

        //min movement velocity already reached
        if (direction == -1 && VelocityX <= -Speed)
            return false;

        return true;
    }

    public override void MoveIn(Vector2 direction)
        => _direction.x = SignZero(direction.x);

    public void Jump()
    {
        if (_canJump)
            VelocityY = _jumpPower;
    }
}

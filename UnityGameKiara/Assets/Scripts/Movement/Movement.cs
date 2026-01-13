using UnityEngine;

// General movement system for moving rigid bodies in a certain direction, rather than setting their velocity (and losing forces from being pushed).
// Also handles friction (well, not really, but it's used to slow down the velocity) in a way that doesn't get stuck on walls.
// A worm's movement might not use a rigid body so I decided to make this have an abstract position and velocity.
public abstract class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _speed = 15.0f;
    [SerializeField]
    private float _acceleration = 120.0f;
    [SerializeField]
    private float _friction = 60.0f;

    public abstract Vector2 Position { get; set; }
    public abstract Vector2 Velocity { get; set; }

    public float VelocityX
    {
        get => Velocity.x;
        set => Velocity = new(value, Velocity.y);
    }

    public float VelocityY
    {
        get => Velocity.y;
        set => Velocity = new(Velocity.x, value);
    }

    public float Speed { get => _speed; set => _speed = value; }
    public float Acceleration { get => _acceleration; set => _acceleration = value; }
    public float Friction { get => _friction; set => _friction = value; }

    // Attempt to move in given direction.
    // direction is a normalized vector.
    public abstract void MoveIn(Vector2 direction);

    // Attempt to move in the direction of the given position.
    public void MoveInTo(Vector2 position)
        => MoveIn((position - Position).normalized);

    // Can we accelerate into the current direction?
    protected abstract bool CanAccelerate();

    // Stop moving in current direction.
    public void Stop()
        => MoveIn(Vector2.zero);
}

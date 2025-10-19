using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageTrigger : MonoBehaviour
{
    private Collider2D _trigerCollider;

    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private float _pushbackStrength = 5.0f;

    public int Damage { get => _damage; set => _damage = value; }

    public float PushbackStrength { get => _pushbackStrength; set => _pushbackStrength = value; }

    private void Start()
    {
        _trigerCollider = GetComponent<Collider2D>();
    }

    private void PushRigidBodyBack(Rigidbody2D rigidBody)
    {
        //rigid body could already be past the trigger
        Vector2 rigidBodyPrevPos = rigidBody.position - rigidBody.linearVelocity * Time.deltaTime;
        Vector2 closestTriggerPoint = _trigerCollider.ClosestPoint(rigidBodyPrevPos);

        //normalized vector pointing from rigid body's previous position
        //to closest point on trigger collider
        Vector2 normal = (closestTriggerPoint - rigidBodyPrevPos).normalized;

        rigidBody.linearVelocity = -normal * _pushbackStrength;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.Damage(_damage);

            if (collision.gameObject.TryGetComponent(out Rigidbody2D rigidBody))
                PushRigidBodyBack(rigidBody);
        }
    }
}

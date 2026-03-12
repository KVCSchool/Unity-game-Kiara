using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageTrigger : MonoBehaviour
{
    private Collider2D _trigerCollider;

    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private bool _pushback = true;
    [SerializeField]
    private float _pushbackStrength = 5.0f;

    [SerializeField]
    private bool _playerOnly = false;

    public int Damage { get => _damage; set => _damage = value; }

    public bool Pushback { get => _pushback; set => _pushback = value; }
    public float PushbackStrength { get => _pushbackStrength; set => _pushbackStrength = value; }

    public bool PlayerOnly { get => _playerOnly; set => _playerOnly = value; }

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
        if (_playerOnly && !collision.CompareTag("Player"))
            return;

        if (collision.gameObject.TryGetComponent(out DamageHitbox damageHitbox))
        {
            damageHitbox.Damage(_damage);

            if (_pushback && collision.gameObject.TryGetComponent(out Rigidbody2D rigidBody))
                PushRigidBodyBack(rigidBody);
        }
    }
}

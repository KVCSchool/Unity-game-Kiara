using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private float _pushbackStrength = 5.0f;

    public int Damage { get => _damage; set => _damage = value; }

    public float PushbackStrength { get => _pushbackStrength; set => _pushbackStrength = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
            health.Damage(_damage);

        if (collision.gameObject.TryGetComponent(out Rigidbody2D rigidBody))
            rigidBody.linearVelocity = -rigidBody.linearVelocity.normalized * _pushbackStrength;
    }
}

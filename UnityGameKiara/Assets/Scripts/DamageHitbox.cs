using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [SerializeField]
    private Health _health;

    // Health this hitbox is linked to.
    public Health Health { get => _health; set => _health = value; }

    public void Damage(int lives)
        => _health.Damage(lives);
}

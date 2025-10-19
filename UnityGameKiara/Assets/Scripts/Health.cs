using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _lives = 5;
    [SerializeField]
    private int _maxLives = 5;

    private bool _alive = true;

    [SerializeField]
    private UnityEvent<int> _onTakeDamage;
    [SerializeField]
    private UnityEvent _onDeath;

    public int Lives { get => _lives; set => _lives = value; }
    public int MaxLives { get => _maxLives; set => _maxLives = value; }
    public bool Alive { get => _alive; set => _alive = value; }

    public UnityEvent<int> OnTakeDamage { get => _onTakeDamage; }
    public UnityEvent OnDeath { get => _onDeath; }

    public virtual void Die()
    {
        Debug.Log("Died!");
        
        _lives = 0;
        _alive = false;

        _onDeath.Invoke();
    }

    public virtual void Damage(int lives)
    {
        if (!_alive)
            return;

        _lives = Mathf.Max(_lives - lives, 0);
        _onTakeDamage.Invoke(lives);
        Debug.Log($"Lost {lives} lives! Remaining: {_lives}");

        if (_lives == 0)
            Die();
    }
}

using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _current = 5;
    [SerializeField]
    private int _max = 5;

    [SerializeField]
    private float _damageCooldownSeconds = 1.0f; 
    private float _damageCountdownSeconds = 0.0f;

    [SerializeField]
    private UnityEvent<int> _onCurrentChanged;
    [SerializeField]
    private UnityEvent<int> _onMaxChanged;
    [SerializeField]
    private UnityEvent<int> _onTakeDamage;
    [SerializeField]
    private UnityEvent _onDeath;

    public int Current
    { 
        get => _current;
        set
        {
            if (_current == value)
                return;

            int old = _current;

            _current = value;
            _onCurrentChanged.Invoke(_current);

            if (_current < old)
                _onTakeDamage.Invoke(_current - old);

            if (_current == 0)
                Die();
        }
    }

    public int Max
    { 
        get => _max; 
        set
        {
            if (_max != value)
            {
                _max = value;
                _onMaxChanged.Invoke(_max);
            }
        }
    }

    public bool Alive { get => Current > 0; }

    public float DamageCooldownSeconds { get => _damageCooldownSeconds; set => _damageCooldownSeconds = value; }

    public UnityEvent<int> OnCurrentChanged { get => _onCurrentChanged; }
    public UnityEvent<int> OnMaxChanged { get => _onMaxChanged; }
    public UnityEvent<int> OnTakeDamage { get => _onTakeDamage; }
    public UnityEvent OnDeath { get => _onDeath; }

    private void Update()
    {
        if (_damageCountdownSeconds > 0.0f)
            _damageCountdownSeconds -= Time.deltaTime;
    }

    public virtual void Die()
    {
        Debug.Log("Died!");
        
        Current = 0;

        _onDeath.Invoke();
    }

    public virtual void Damage(int lives)
    {
        if (!Alive || _damageCountdownSeconds > 0.0f)
            return;

        Current = Mathf.Max(Current - lives, 0);
        _damageCountdownSeconds = _damageCooldownSeconds;
    }
}

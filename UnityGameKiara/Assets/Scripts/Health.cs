using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _lives = 5;
    [SerializeField]
    private int _maxLives = 5;

    private bool _alive = true;

    public int Lives { get => _lives; set => _lives = value; }
    public int MaxLives { get => _maxLives; set => _maxLives = value; }
    public bool Alive { get => _alive; set => _alive = value; }

    public virtual void Die()
    {
        _lives = 0;
        _alive = false;

        Debug.Log("Died!");
    }

    public virtual void Damage(int lives)
    {
        if (!_alive)
            return;

        _lives = Mathf.Max(_lives - lives, 0);
        Debug.Log($"Lost {lives} lives! Remaining: {_lives}");

        if (_lives == 0)
            Die();
    }
}

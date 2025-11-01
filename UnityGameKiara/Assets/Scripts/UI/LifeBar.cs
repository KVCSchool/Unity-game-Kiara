using System.Collections.Generic;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
    // Health to track.
    [SerializeField]
    private Health _health = null;

    private readonly List<GameObject> _lifeIcons = new();

    [SerializeField]
    private GameObject _lifeIcon;

    // Health to track.
    public Health Health 
    { 
        get => _health;
        set
        {
            if (_health == value)
                return;

            if (_health != null)
                FiniHealth();

            _health = value;

            InitHealth();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _lifeIcon.SetActive(false);

        if (_health != null)
            InitHealth();
    }

    private void InitHealth()
    {
        _health.OnCurrentChanged.AddListener(OnCurrentChanged);
        _health.OnMaxChanged.AddListener(OnMaxChanged);

        DestroyAllLifeIcons();
        CreateLifeIcons();
        UpdateLifeIcons();
    }

    private void FiniHealth()
    {
        _health.OnCurrentChanged.RemoveListener(OnCurrentChanged);
        _health.OnMaxChanged.RemoveListener(OnMaxChanged);
    }

    private void DestroyAllLifeIcons()
    {
        foreach (GameObject lifeIcon in _lifeIcons)
            Destroy(lifeIcon);

        _lifeIcons.Clear();
    }

    private void CreateLifeIcons()
    {
        for (int i = 0; i < _health.Max; i++)
            _lifeIcons.Add(Instantiate(_lifeIcon, transform));
    }

    private void UpdateLifeIcons()
    {
        for (int i = 0; i < _lifeIcons.Count; i++)
            _lifeIcons[i].SetActive(i < _health.Current);
    }

    private void OnCurrentChanged(int current)
        => UpdateLifeIcons();

    private void OnMaxChanged(int max)
    {
        DestroyAllLifeIcons();
        CreateLifeIcons();
        UpdateLifeIcons();
    }
}

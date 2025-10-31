using System.Collections.Generic;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
    // Health to track.
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
            {
                _health.OnCurrentChanged.RemoveListener(OnCurrentChanged);
                _health.OnMaxChanged.RemoveListener(OnMaxChanged);
            }

            _health = value;

            _health.OnCurrentChanged.AddListener(OnCurrentChanged);
            _health.OnMaxChanged.AddListener(OnMaxChanged);

            OnMaxChanged(_health.Max);
            OnCurrentChanged(_health.Current);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _lifeIcon.SetActive(false);
    }

    private void DestroyAllLifeIcons()
    {
        foreach (GameObject lifeIcon in _lifeIcons)
            Destroy(lifeIcon);

        _lifeIcons.Clear();
    }

    private void CreateLifeIcons(int num)
    {
        for (int i = 0; i < num; i++)
            _lifeIcons.Add(Instantiate(_lifeIcon, transform));
    }

    private void OnCurrentChanged(int current)
    {
        for (int i = 0; i < _lifeIcons.Count; i++)
            _lifeIcons[i].SetActive(i < current);
    }

    private void OnMaxChanged(int max)
    {
        DestroyAllLifeIcons();
        CreateLifeIcons(max);
    }
}

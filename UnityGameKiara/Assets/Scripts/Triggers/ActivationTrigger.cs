using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ActivationTrigger : MonoBehaviour
{
    private bool _active = false;

    private bool _activatedOnce = false;

    // Whether this trigger should only be able to be activated once ever.
    [SerializeField]
    private bool _once = false;

    // Leave empty if all tags are allowed.
    [SerializeField]
    private string _allowedTag = "";

    // Colliders keeping this trigger active.
    private readonly List<Collider2D> _activators = new();

    [SerializeField]
    private UnityEvent _onActivate;
    [SerializeField]
    private UnityEvent _onDeactivate;

    // Is this trigger active?
    public bool Active { get => _active; }
    
    // Whether this trigger should only be able to be activated once ever.
    public bool Once { get => _once; }

    // Leave empty if all tags are allowed.
    public string AllowedTag { get => _allowedTag; set => _allowedTag = value; }

    public UnityEvent OnActivate { get => _onActivate; }
    public UnityEvent OnDeactivate { get => _onDeactivate; }

    public void Activate()
    {
        //don't activate if already active
        //nor activate more than once if requested
        if (_active || (_once && _activatedOnce))
            return;

        _active = true;
        _activatedOnce = true;
        _onActivate.Invoke();
    }

    public void Deactivate()
    {
        //already inactive
        if (!_active)
            return;

        _active = false;
        _onDeactivate.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if allowed tag is not empty, only allow game objects with said tag
        //to activate this trigger
        if (_allowedTag != string.Empty && !collision.CompareTag(_allowedTag))
            return;

        _activators.Add(collision);
        Activate();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool removed = _activators.Remove(collision);

        if (removed && _activators.Count == 0)
            Deactivate();
    }
}

using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    private RespawnController _respawnController;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private UnityEvent<Checkpoint> _onActivate;

    public Transform SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }

    public UnityEvent<Checkpoint> OnActivate { get => _onActivate; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _respawnController = GameObject.FindGameObjectWithTag("RespawnController").GetComponent<RespawnController>();
    }

    public void Activate()
    {
        if (_respawnController.SpawnPoint == _spawnPoint.position)
            return;

        _respawnController.SpawnPoint = _spawnPoint.position;
        _onActivate.Invoke(this);

        Debug.Log("Checkpoint activated!");
    }
}

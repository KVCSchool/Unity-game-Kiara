using UnityEngine;
using UnityEngine.Events;

public class RespawnController : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private Vector3 _spawnPoint = Vector3.zero;
    [SerializeField]
    private float _respawnDelaySeconds;

    private GameObject _player;
    private bool _playerAlive = false;
    private float _playerDeathDeltaTime = 0.0f;

    [SerializeField]
    private UnityEvent<GameObject> _onPlayerSpawn;

    public Vector3 SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }
    public float RespawnDelaySeconds { get => _respawnDelaySeconds; set => _respawnDelaySeconds = value; }

    public UnityEvent<GameObject> OnPlayerSpawn { get => _onPlayerSpawn; }

    // Update is called once per frame
    private void Update()
    {
        if (!_playerAlive)
        {
            _playerDeathDeltaTime += Time.deltaTime;

            if (_playerDeathDeltaTime >= _respawnDelaySeconds)
                RespawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        _player = Instantiate(_playerPrefab, _spawnPoint, new());
        _playerAlive = true;

        Health playerHealth = _player.GetComponent<Health>();
        playerHealth.OnDeath.AddListener(OnPlayerDeath);

        _onPlayerSpawn.Invoke(_player);
    }

    public void RespawnPlayer()
    {
        Destroy(_player);

        SpawnPlayer();
    }

    private void OnPlayerDeath()
    {
        _playerAlive = false;
        _playerDeathDeltaTime = 0.0f;
    }
}

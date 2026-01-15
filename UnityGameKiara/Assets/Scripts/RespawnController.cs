using UnityEngine;
using UnityEngine.Events;

public class RespawnController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _spawnPoint = Vector3.zero;
    [SerializeField]
    private float _respawnDelaySeconds;

    [SerializeField]
    private GameObject _player;
    private Health _playerHealth;
    private PlayerController _playerController;
    private bool _playerAlive = false;
    private float _playerRespawnCooldown = 0.0f;

    [SerializeField]
    private UnityEvent<GameObject> _onPlayerSpawn;

    public Vector3 SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }
    public float RespawnDelaySeconds { get => _respawnDelaySeconds; set => _respawnDelaySeconds = value; }

    public UnityEvent<GameObject> OnPlayerSpawn { get => _onPlayerSpawn; }

    private void Start()
    {
        _player.SetActive(false);
        _playerHealth = _player.GetComponent<Health>();
        _playerController = _player.GetComponent<PlayerController>();
        _playerHealth.OnDeath.AddListener(OnPlayerDeath);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_playerAlive)
        {
            _playerRespawnCooldown -= Time.deltaTime;

            if (_playerRespawnCooldown <= 0.0f)
                SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        _player.SetActive(true);
        _player.transform.position = _spawnPoint;

        _playerAlive = true;
        _playerHealth.Current = _playerHealth.Max;

        _playerController.ResetState();

        _onPlayerSpawn.Invoke(_player);
    }

    private void OnPlayerDeath()
    {
        _playerAlive = false;
        _playerRespawnCooldown = RespawnDelaySeconds;
    }
}

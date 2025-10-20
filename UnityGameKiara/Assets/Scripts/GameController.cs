using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerCamera _playerCamera;
    [SerializeField]
    private RespawnController _respawnController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SceneManager.LoadScene("TestArea", LoadSceneMode.Additive);

        _respawnController.OnPlayerSpawn.AddListener(OnPlayerSpawn);
        _respawnController.SpawnPlayer();
    }

    private void OnPlayerSpawn(GameObject player)
    {
        _playerCamera.Follow = player.GetComponent<Rigidbody2D>();
    }
}

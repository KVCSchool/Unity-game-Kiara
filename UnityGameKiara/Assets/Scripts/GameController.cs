using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private RespawnController _respawnController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SceneManager.LoadScene("TestArea", LoadSceneMode.Additive);

        _respawnController.SpawnPlayer();
    }
}

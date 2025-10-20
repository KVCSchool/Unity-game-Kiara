using UnityEngine;
using UnityEngine.SceneManagement;

// This will probably be temporary, but you know what they say:
// there's nothing more permanent than a temporary solution.
public class SceneLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene("TestArea", LoadSceneMode.Additive);
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
    }
}

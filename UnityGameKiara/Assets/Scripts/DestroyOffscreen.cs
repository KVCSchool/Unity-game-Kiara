using UnityEngine;

// If this sprite renderer goes offscreen, destroy a specific game object.
[RequireComponent(typeof(SpriteRenderer))]
public class DestroyOffscreen : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameObject;

    private void OnBecameInvisible()
    {
        if (enabled)
            Destroy(_gameObject);
    }
}

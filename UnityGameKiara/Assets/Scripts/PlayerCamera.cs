using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    //TODO: option to offset camera in player's movement direction
    //TODO: tweening for switching between camera modes & changing offset
    //TODO: mode for fitting a rectangle on screen (least important, could just design around this)

    private Camera _camera;

    [SerializeField]
    private Transform _follow;

    // Transform to follow.
    public Transform Follow { get => _follow; set => _follow = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(_follow.position.x, _follow.position.y, transform.position.z);
    }
}

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    //TODO: tweening for switching between camera modes & changing offset
    //TODO: camera bounds
    //TODO: mode for fitting a rectangle on screen (least important, could just design around this)

    [SerializeField]
    private Rigidbody2D _follow;

    [SerializeField]
    private Vector3 _offset = Vector3.zero;
    private Vector3 _offsetVelocity = Vector3.zero;

    // Smoothing times per axis for offset smooth damping.
    [SerializeField]
    private Vector3 _smoothTimes = new(0.3f, 2.0f, 0.0f);

    // Rigid body to follow.
    public Rigidbody2D Follow { get => _follow; set => _follow = value; }

    

    private Vector3 SmoothDampVector3(Vector3 current, Vector3 target, ref Vector3 velocity, Vector3 smoothTimes)
    {
        return new(
            Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTimes.x),
            Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTimes.y),
            Mathf.SmoothDamp(current.z, target.z, ref velocity.z, smoothTimes.z)
            );
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 targetOffset = new(0.0f, 0.0f, -10.0f);

        if (_follow.linearVelocityX != 0.0f)
            targetOffset.x = Mathf.Sign(_follow.linearVelocityX) * 6.0f;

        if (_follow.linearVelocityY != 0.0f)
            targetOffset.y = Mathf.Sign(_follow.linearVelocityY) * 3.0f;

        _offset = SmoothDampVector3(_offset, targetOffset, ref _offsetVelocity, _smoothTimes);

        transform.position = new Vector3(_follow.position.x, _follow.position.y) + _offset;
    }
}

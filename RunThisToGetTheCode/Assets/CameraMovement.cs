
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [FormerlySerializedAs("Player")] public Transform player;
    [FormerlySerializedAs("CameraFollowSpeed")] public float cameraFollowSpeed;
    private float _speed;

    private void Start()
    {
        _speed = cameraFollowSpeed;
        Vector3 moveTo = player.position;
        //Subtract to have cam in front of elements
        moveTo.z = -1;
        transform.position = moveTo;
    }

    private void Update()
    {
        Vector3 moveTo = player.position;
        //Lerp linear inerpolation to smooth transition
        moveTo.z = -1;
        transform.position = Vector3.Lerp(transform.position,moveTo,Time.deltaTime*_speed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerToPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public Vector3 offsetForTop;
    public Vector3 currentVelocity;

    private Quaternion topRot;

    private Quaternion normalRot;

    public float smooth;
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        normalRot = transform.rotation;
        topRot = Quaternion.Euler(90, 0, 0);

    }

    private void LateUpdate()
    {

        if (Physics.Linecast(player.position + offset + new Vector3(0, 2, 0), player.position, 1 << 8))
        {
            Vector3 target = player.position + offsetForTop;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smooth, maxSpeed, Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, topRot, 0.2f * Time.deltaTime);
        }
        else
        {
            Vector3 target = player.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smooth, maxSpeed, Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, normalRot, 0.2f * Time.deltaTime);
        }
    }
}

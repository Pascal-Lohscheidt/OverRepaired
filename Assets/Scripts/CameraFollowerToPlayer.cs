using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerToPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public Vector3 currentVelocity;
    public float smooth;
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 target = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smooth, maxSpeed, Time.deltaTime);
    }
}

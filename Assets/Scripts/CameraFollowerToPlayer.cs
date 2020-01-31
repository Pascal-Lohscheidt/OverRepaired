using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerToPlayer : MonoBehaviour
{
    public Transform Player;
    public Vector3 Offfset;
    public Vector3 currentVelocity;
    public float Smooth;
    public float MaxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 target = Player.position + Offfset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, Smooth, MaxSpeed, Time.deltaTime);
    }
}

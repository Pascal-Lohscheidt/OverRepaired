using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerToPlayer : Singleton<CameraFollowerToPlayer>
{
    public Transform player;
    public Vector3 offset;
    public Vector3 offsetForTop;
    public Vector3 currentVelocity;

    private Quaternion topRot;

    private Quaternion normalRot;

    private bool shouldZoomIn;

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

        if (shouldZoomIn)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 15, 2f * Time.deltaTime);
        else
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, 3f * Time.deltaTime);
    }

    public void ToggleZoomBehaviour(bool toggle)
    {
        shouldZoomIn = toggle;
    }

}

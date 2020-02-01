using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAbleObject : MonoBehaviour
{
    private new Renderer renderer;
    private Rigidbody rigidbody;


    private void Start()
    {
        renderer = GetComponent<Renderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void ToggleVisibility(bool visible)
    {
        renderer.enabled = visible;
    }

    public void SetAffectedByGravity(bool usesGravity)
    {
        rigidbody.useGravity = usesGravity;

        if(!usesGravity)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}

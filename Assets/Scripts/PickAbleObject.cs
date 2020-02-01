using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAbleObject : MonoBehaviour
{
    private new Renderer renderer;
    private Rigidbody rigidbody;
    public string partName;


    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private HUDIconHandler iconHandler;

    protected virtual void Start()
    {
        iconHandler = GetComponent<HUDIconHandler>();
        iconHandler.UpdateText(partName);
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

using UnityEngine;

public class PickAbleObject : MonoBehaviour
{
    private new Renderer renderer;
    public bool pickable;
    private Rigidbody rigidbody;
    public string partName;
    protected HUDIconHandler iconHandler;

    private void Awake()
    {
        pickable = true;
        renderer = GetComponent<Renderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        iconHandler = GetComponent<HUDIconHandler>();
        iconHandler.UpdateText(partName);
    }

    public void ToggleVisibility(bool visible)
    {
        renderer.enabled = visible;
    }

    public void SetUnpickable()
    {
        pickable = false;
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

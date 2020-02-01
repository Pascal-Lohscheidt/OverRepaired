using UnityEngine;

public class BreakableObject : InteractableObject
{
    public string objectName;
    public bool working = true;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Resource.ResourceType resourceType;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
        FixObject();
        isContinuouslyInteractlable = false;
        // You can look up the property by ID instead of the string to be more efficient.
    }

    public void FixObject()
    {
        renderer.material.SetColor("_BaseColor", Color.green);
        working = true;
    }

    public void BreakObject()
    {
        print("Broken: " + objectName);
        renderer.material.SetColor("_BaseColor", Color.red);

        IssueManager.Instance.CreateIssue(this); //Creating a new Issue because this component Broke
        working = false;
    }

    public bool IsBroken()
    {
        return !working;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerInteractionHandler handler = other.gameObject.GetComponent<PlayerInteractionHandler>();
            handler.EnterInteractionArea(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInteractionHandler handler = other.gameObject.GetComponent<PlayerInteractionHandler>();
            handler.LeaveInteractionArea();
        }
    }


    public override void InteractOnce()
    {
        base.InteractOnce();
        FixObject();
    }

    public override void InteractContinuously()
    {
    }
}

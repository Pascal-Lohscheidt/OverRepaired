using UnityEngine;

public class BreakableObject : InteractableObject
{
    public string objectName;
    public bool working = true;
    [SerializeField] private Renderer renderer;
    public Issue currentIssue;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
        renderer.material.SetColor("_BaseColor", Color.green);
        working = true;
        isContinuouslyInteractlable = false;
        // You can look up the property by ID instead of the string to be more efficient.
    }

    public void FixObject()
    {
        IssueManager.Instance.IssueFixed(this); //Creating a new Issue because this component Broke
        renderer.material.SetColor("_BaseColor", Color.green);
        working = true;
    }

    public void BreakObject()
    {
        print("Broken: " + objectName);
        renderer.material.SetColor("_BaseColor", Color.red);

        currentIssue = IssueManager.Instance.CreateIssue(this); //Creating a new Issue because this component Broke
        working = false;
    }

    public bool IsBroken()
    {
        return !working;
    }

    public override void InteractOnce(PlayerInteractionHandler handler)
    {
        base.InteractOnce(handler);
    }

    public override bool AddPickableComponent(PickAbleObject pickAbleObject)
    {
        if(pickAbleObject is ConstructedRepairComponent)
        {
            ConstructedRepairComponent newComponent = (ConstructedRepairComponent)pickAbleObject;
            if (currentIssue.seekedWord.ToString() == newComponent.componentName)
            {
                FixObject();
                return true;
            }
            return false;
        }
        return false;
        
    }
}

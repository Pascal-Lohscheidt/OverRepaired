using UnityEngine;

public class BreakableObject : InteractableObject
{
    public string objectName;
    public bool working = true;
    [SerializeField] public Resource.ResourceType resourceType;
    public Issue currentIssue;
    public float lastTimeFixed;
    private HUDIconHandler iconHandler;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
        //renderer.material.SetColor("_BaseColor", Color.green);
        working = true;
        isContinuouslyInteractlable = false;
        iconHandler = gameObject.AddComponent<HUDIconHandler>();
        iconHandler.dependsOnDistance = false;
        // You can look up the property by ID instead of the string to be more efficient.
    }

    public void FixObject()
    {
        IssueManager.Instance.IssueFixed(this); //Creating a new Issue because this component Broke
        //renderer.material.SetColor("_BaseColor", Color.green);
        working = true;
        iconHandler.HideText();
        lastTimeFixed = Time.unscaledTime;
    }

    public void BreakObject()
    {
        print("Broken: " + objectName);
        //renderer.material.SetColor("_BaseColor", Color.red);
        iconHandler.UpdateText("FixMe");
        iconHandler.SetTextSize(45f);
        iconHandler.SetColor(Color.red);
        iconHandler.ShowText();
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
            if (currentIssue.seekedWord.ToString() == newComponent.partName)
            {
                Destroy(newComponent.gameObject);
                FixObject();
                return true;
            }
            return false;
        }
        return false;
        
    }
}

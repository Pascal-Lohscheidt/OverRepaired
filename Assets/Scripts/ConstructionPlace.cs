using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConstructionPlace : InteractableObject
{
    public enum ConstructionPhase { Empty, Loaded, Constructing, Done}  //red, yellow, n.a, green
    public ConstructionPhase currentPhase;
    public int maxAmountOfComponents;
    public float constructionDuration;

    public Sprite partsRequiredSprite;
    public Sprite doneSprite;
    [SerializeField] private Animator anim;

    [HideInInspector] public List<RepairComponent> addedComponents;
    private ConstructedRepairComponent finishedRepairComponent;
    [SerializeField] private GameObject constructedComponentPrefab;
    private float constructionTimer;
    [SerializeField] private Transform boxParent;

    //[SerializeField] private Renderer renderer; 


    // Start is called before the first frame update
    void Start()
    {
        ChangePhase(ConstructionPhase.Empty);
        anim = GetComponent<Animator>();
        anim.enabled = false;
        isContinuouslyInteractlable = true;
    }

    /// <summary>
    /// Returns false if the table is full already or the component type already exists.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool AddComponentToConstructionPlace(RepairComponent component)
    {
        if(addedComponents.Count < maxAmountOfComponents)
        {
            component.SetUnpickable();
            addedComponents.Add(component);
            component.HideHUD();
            component.transform.SetParent(boxParent);
            component.transform.position = boxParent.position;
            component.transform.rotation = boxParent.rotation;
            component.SetAffectedByGravity(false);
            ChangePhase(ConstructionPhase.Loaded);
            return true;
        }
       
        return false;
    }

    /// <summary>
    /// Returns true if the Construction is finished and then it links the components
    /// </summary>
    /// <returns></returns>
    public bool Construct()
    {
        if (currentPhase != ConstructionPhase.Done)
        {
            constructionTimer += Time.deltaTime;
            anim.enabled = true;
            CameraFollowerToPlayer.Instance.ToggleZoomBehaviour(true);
            ChangePhase(ConstructionPhase.Constructing);
            if (constructionTimer >= constructionDuration)
            {
                FinishConstruction();
                return true;
            }
        }
        return false;
    }

    private void FinishConstruction()
    {
        ChangePhase(ConstructionPhase.Done);
        constructionTimer = 0;

        anim.enabled = false;
        anim.Rebind();
        CameraFollowerToPlayer.Instance.ToggleZoomBehaviour(false);
        //anim.enabled = false;
        finishedRepairComponent = Instantiate(constructedComponentPrefab, boxParent.position, boxParent.rotation, null).GetComponent<ConstructedRepairComponent>();

        foreach (RepairComponent comp in addedComponents)
        {
            comp.transform.SetParent(finishedRepairComponent.transform);
            comp.transform.position = finishedRepairComponent.transform.position;
            comp.transform.rotation = finishedRepairComponent.transform.rotation;
            Destroy(comp.GetComponent<Rigidbody>());
            Destroy(comp);
        }

        string cName = "";

        if (IssueManager.Instance.currentIssueList.Values.ToList().Exists(i => i.ComponentsMatchIssue(addedComponents)))
        {
            cName = IssueManager.Instance.currentIssueList.Values.ToList().Find(i => i.ComponentsMatchIssue(addedComponents)).ReturnProperName();
        }
        else
        {
            IssueManager.Instance.CaseCompindingNotNeeded(addedComponents);
            foreach (RepairComponent item in addedComponents) cName += item.partName;
        }

        finishedRepairComponent.partName = cName;
    }

    public void CancelConstruction()
    {
        constructionTimer = 0;
        anim.enabled = false;
        anim.Rebind();
        CameraFollowerToPlayer.Instance.ToggleZoomBehaviour(false);

        if (currentPhase != ConstructionPhase.Done)
        {
            if (addedComponents.Count > 0)
                ChangePhase(ConstructionPhase.Loaded);
            else
                ChangePhase(ConstructionPhase.Empty);
        }
    }


    //================= Overide Methods =====================

    public override bool AddPickableComponent(PickAbleObject pickAbleObject)
    {
        return AddComponentToConstructionPlace((RepairComponent)pickAbleObject);
    }

    public override void InteractContinuously(PlayerInteractionHandler handler)
    {
        if (addedComponents.Count > 0)
        {
            base.InteractContinuously(handler);
            bool done = Construct();
            if (done) handler.FinishContiniousInteraction();
        }
    }

    public override void CancelContinuousInteraction()
    {
        base.CancelContinuousInteraction();
        CancelConstruction();
    }

    public override ConstructedRepairComponent GetRepairComponent()
    {
        if(finishedRepairComponent != null)
        {
            ConstructedRepairComponent returnComponent = finishedRepairComponent;

            finishedRepairComponent = null;
            addedComponents.Clear();
            ChangePhase(ConstructionPhase.Empty);
            return returnComponent;
        }
        return null;
    }

    private void ChangePhase(ConstructionPhase newPhase)
    {
        currentPhase = newPhase;
    }

}

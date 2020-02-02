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

    [HideInInspector] public List<RepairComponent> addedComponents;
    private ConstructedRepairComponent finishedRepairComponent;
    [SerializeField] private GameObject constructedComponentPrefab;
    private float constructionTimer;

    //[SerializeField] private Renderer renderer; 


    // Start is called before the first frame update
    void Start()
    {
        ChangePhase(ConstructionPhase.Empty);
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
            addedComponents.Add(component);
            component.gameObject.GetComponent<Renderer>().enabled = false;
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
        finishedRepairComponent = Instantiate(constructedComponentPrefab, transform).GetComponent<ConstructedRepairComponent>();

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
        //switch (newPhase)
        //{
        //    case ConstructionPhase.Empty:
        //        renderer.material.SetColor("_BaseColor", Color.red);
        //        break;
        //    case ConstructionPhase.Loaded:
        //        renderer.material.SetColor("_BaseColor", Color.yellow);
        //        break;
        //    case ConstructionPhase.Constructing:
        //        renderer.material.SetColor("_BaseColor", Color.blue);
        //        break;
        //    case ConstructionPhase.Done:
        //        renderer.material.SetColor("_BaseColor", Color.green);
        //        break;
        //}
    }

}

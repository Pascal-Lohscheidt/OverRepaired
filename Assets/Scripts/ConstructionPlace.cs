using UnityEngine;
using System.Collections.Generic;

public class ConstructionPlace : InteractableObject
{
    public enum ConstructionPhase { Empty, Loaded, Constructing, Done}  //red, yellow, n.a, green
    public ConstructionPhase currentPhase;
    public int maxAmountOfComponents;
    public float constructionDuration;

    public Sprite partsRequiredSprite;
    public Sprite doneSprite;

    [HideInInspector] public List<RepairComponent> addedComponents;
    private float constructionTimer;

    [SerializeField] private Renderer renderer; 


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
        constructionTimer += Time.deltaTime;
        ChangePhase(ConstructionPhase.Constructing);
        if (constructionTimer >= constructionDuration)
        {
            FinishConstruction();
            return true;
        }
        return false;
    }

    private void FinishConstruction()
    {
        ChangePhase(ConstructionPhase.Done);
        //TODO: Add instancaite method

    }

    public void CancelConstruction()
    {
        constructionTimer = 0;
        ChangePhase(ConstructionPhase.Loaded);
    }

    public override void InteractContinuously(PlayerInteractionHandler handler)
    {
        base.InteractContinuously(handler);
        bool done = Construct();
        if (done) handler.FinishContiniousInteraction(); 

    }

    private void ChangePhase(ConstructionPhase newPhase)
    {
        currentPhase = newPhase;
        switch (newPhase)
        {
            case ConstructionPhase.Empty:
                renderer.material.SetColor("_BaseColor", Color.red);
                break;
            case ConstructionPhase.Loaded:
                renderer.material.SetColor("_BaseColor", Color.yellow);
                break;
            case ConstructionPhase.Constructing:
                renderer.material.SetColor("_BaseColor", Color.blue);
                break;
            case ConstructionPhase.Done:
                renderer.material.SetColor("_BaseColor", Color.green);
                break;
        }
    }

}

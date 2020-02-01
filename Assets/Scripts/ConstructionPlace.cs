using UnityEngine;
using System.Collections.Generic;

public class ConstructionPlace : MonoBehaviour
{
    public enum ConstructionPhase { Empty, Loaded, Constructing, Done}
    public ConstructionPhase currentPhase;
    public int maxAmountOfComponents;
    public float constructionDuration;

    public Sprite partsRequiredSprite;
    public Sprite doneSprite;

    [HideInInspector] public List<RepairComponent> addedComponents;
    private float constructionTimer;


    // Start is called before the first frame update
    void Start()
    {
        currentPhase = ConstructionPhase.Empty;
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
            currentPhase = ConstructionPhase.Loaded;
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
        if(constructionTimer >= constructionDuration)
        {
            FinishConstruction();
            return true;
        }
        return false;
    }

    private void FinishConstruction()
    {
        currentPhase = ConstructionPhase.Done;
        //TODO: Add instancaite method

    }

    public void CancelConstruction()
    {
        constructionTimer = 0;
        currentPhase = ConstructionPhase.Loaded;
    }

}

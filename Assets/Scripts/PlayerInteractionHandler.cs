using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TODO: Create a user Controls facade and extract the keyhandler code;
/// </summary>
public class PlayerInteractionHandler : MonoBehaviour
{
    private bool isInInteractionArea;
    private InteractableObject currentObject;
    private bool doneWithCurrentInteraction; //only intersting for continous interactions
    private PickAbleObject holdingComponent;
    [SerializeField] private Transform holdingTransform;


    public void Update()
    {
        if (currentObject != null)
        {
            if (currentObject.isContinuouslyInteractlable)
            {
                if (Input.GetKey(KeyCode.E) && isInInteractionArea && !doneWithCurrentInteraction)
                    currentObject.InteractContinuously(this);
                else
                    currentObject.CancelContinuousInteraction();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E) && isInInteractionArea)
                    currentObject.InteractOnce(this);
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(holdingComponent == null)
            {
                PickUpComponent();
            }
            else
            {
                DropComponent();
            }
        }
    }


    public void EnterInteractionArea(InteractableObject interactableObject)
    {
        currentObject = interactableObject;
        isInInteractionArea = true;
    }

    public void LeaveInteractionArea()
    {
        currentObject = null;
        isInInteractionArea = false;
        doneWithCurrentInteraction = false;

    }

    private void PickUpComponent()
    {
        if (currentObject == null)
        {
            List<PickAbleObject> componentsInScene = new List<PickAbleObject>();
            componentsInScene.AddRange(FindObjectsOfType<PickAbleObject>());
            componentsInScene.RemoveAll(i => Vector3.Distance(i.transform.position, transform.position) > 3f);

            PickAbleObject componentToPick = null;

            if (componentsInScene.Count > 0)
                componentToPick = componentsInScene.OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).First();

            if (componentToPick != null)
            {
                holdingComponent = componentToPick;
                holdingComponent.transform.SetParent(transform);
                holdingComponent.ToggleVisibility(true);
                holdingComponent.transform.position = holdingTransform.position;
                holdingComponent.SetAffectedByGravity(false);
            }
        }
        else
        {
            holdingComponent = currentObject.GetRepairComponent();
            if (holdingComponent != null)
            {
                holdingComponent.transform.SetParent(transform);
                holdingComponent.ToggleVisibility(true);
                holdingComponent.transform.position = holdingTransform.position;
                holdingComponent.SetAffectedByGravity(false);
            }
        }


        //component.gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void DropComponent()
    {
        if(currentObject == null) //just drop it on the floor
        {
            holdingComponent.transform.SetParent(null);
            holdingComponent.ToggleVisibility(true);
            holdingComponent.SetAffectedByGravity(true);
        }
        else if(currentObject.AddPickableComponent(holdingComponent)) 
        {
            Destroy(holdingComponent.gameObject);
        }

        holdingComponent = null;
    }

    public void FinishContiniousInteraction()
    {
        doneWithCurrentInteraction = true;
    }

}

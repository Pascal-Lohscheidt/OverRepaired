using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Create a user Controls facade and extract the keyhandler code;
/// </summary>
public class PlayerInteractionHandler : MonoBehaviour
{
    private bool isInInteractionArea;
    private InteractableObject currentObject;
    private bool doneWithCurrentInteraction; //only intersting for continous interactions
    private RepairComponent holdingComponent;

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

    }

    private void DropComponent()
    {
        if(currentObject == null) //just drop it on the floor
        {

        }
        else
        {

        }
    }

    public void Update()
    {
        if (currentObject != null)
        {
            if (currentObject.isContinuouslyInteractlable)
            {
                if (Input.GetKey(KeyCode.E) && isInInteractionArea && !doneWithCurrentInteraction)
                    currentObject.InteractContinuously(this);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E) && isInInteractionArea)
                    currentObject.InteractOnce();
            }
        }
    }

    public void FinishContiniousInteraction()
    {
        doneWithCurrentInteraction = true;
    }

}

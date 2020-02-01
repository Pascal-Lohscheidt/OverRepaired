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

    public void EnterInteractionArea(InteractableObject interactableObject)
    {
        currentObject = interactableObject;
        isInInteractionArea = true;
    }

    public void LeaveInteractionArea(InteractableObject interactableObject)
    {
        currentObject = null;
        isInInteractionArea = false;
    }


    public void Update()
    {
        if(!currentObject.isContinuouslyInteractlable)
        {

        }

        if(Input.GetKey(KeyCode.A) && currentObject != null && isInInteractionArea)
        {
            currentObject.InteractOnce();
        }
    }
}

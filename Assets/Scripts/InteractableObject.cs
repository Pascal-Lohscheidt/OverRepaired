using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [HideInInspector] public bool isContinuouslyInteractlable;

    public virtual void InteractOnce()
    {

    }

    public virtual void InteractContinuously(PlayerInteractionHandler handler)
    {

    }

}

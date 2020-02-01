using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [HideInInspector] public bool isContinuouslyInteractlable;
    

    public virtual void InteractOnce(PlayerInteractionHandler handler) { }

    public virtual void InteractContinuously(PlayerInteractionHandler handler) { }

    public virtual void CancelContinuousInteraction()
    {

    }

    public virtual bool AddPickableComponent(PickAbleObject pickAbleObject) => false;

    public virtual ConstructedRepairComponent GetRepairComponent() => null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInteractionHandler handler = other.gameObject.GetComponent<PlayerInteractionHandler>();
            handler.EnterInteractionArea(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInteractionHandler handler = other.gameObject.GetComponent<PlayerInteractionHandler>();
            handler.LeaveInteractionArea();
        }
    }

}

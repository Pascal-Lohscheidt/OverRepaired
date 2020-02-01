using UnityEngine;

public class RepairComponent : PickAbleObject
{
    public string partName;
    private HUDIconHandler iconHandler;

    private void Start()
    {
        iconHandler = GetComponent<HUDIconHandler>();
        iconHandler.UpdateText(partName);
    }
}

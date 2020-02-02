using UnityEngine;

public class ConstructedRepairComponent : PickAbleObject
{

    public void SetName(string newName)
    {
        partName = newName;
    }

    protected override void Start()
    {
        base.Start();
        print("test");
        ToggleVisibility(false);
        SetAffectedByGravity(false);
    }
}

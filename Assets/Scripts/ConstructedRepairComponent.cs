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
        transform.localScale = new Vector3(0.5f, 1, 1);
    }
}

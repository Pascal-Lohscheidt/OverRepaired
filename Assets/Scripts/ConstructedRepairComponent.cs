using UnityEngine;

public class ConstructedRepairComponent : PickAbleObject
{
    public string componentName;

    public void SetName(string newName)
    {
        componentName = newName;
    }

    private void Start()
    {
        print("test");
        ToggleVisibility(false);
        SetAffectedByGravity(false);
        transform.localScale = new Vector3(1, 1, 1);
    }
}

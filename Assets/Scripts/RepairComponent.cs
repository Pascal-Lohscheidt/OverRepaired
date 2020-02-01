using System.Collections.Generic;
using UnityEngine;

public class RepairComponent : PickAbleObject
{
    public string partName;
<<<<<<< HEAD
    public static List<RepairComponent> Instances { get; private set; } = new List<RepairComponent>();
    int instanceIndex = 0;

    protected virtual void OnEnable()
    {
        instanceIndex = Instances.Count;
        Instances.Add(this);
    }

    protected virtual void OnDisable()
    {
        if (instanceIndex < Instances.Count)
        {
            var end = Instances.Count - 1;
            Instances[instanceIndex] = Instances[end];
            Instances.RemoveAt(end);
        }
=======
    private HUDIconHandler iconHandler;

    private void Start()
    {
        iconHandler = GetComponent<HUDIconHandler>();
        iconHandler.UpdateText(partName);
>>>>>>> babd530a77f0c77206906c4223e77a3fd749dad2
    }
}

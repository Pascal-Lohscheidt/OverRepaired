using System.Collections.Generic;
using UnityEngine;

public class RepairComponent : PickAbleObject
{
    public string partName;
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
    }
}

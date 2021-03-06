﻿using System.Collections.Generic;
using UnityEngine;

public class RepairComponent : PickAbleObject
{
    public static List<RepairComponent> Instances { get; private set; } = new List<RepairComponent>();
    public enum ComponentType { Bottom, Middle, Top};
    public ComponentType componentType;
    int instanceIndex = 0;

    protected virtual void OnEnable()
    {
        instanceIndex = Instances.Count;
        Instances.Add(this);
    }
    protected override void Start()
    {
        base.Start();
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

    public void HideHUD()
    {
        iconHandler.HideText();
        iconHandler.dependsOnDistance = false;
    }
}
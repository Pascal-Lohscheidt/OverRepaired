using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehavioids which inherit this class will be tracked in the static 
/// Instances property.
/// </summary>
/// <typeparam name="T"></typeparam>
public class InstanceTracker<T> : MonoBehaviour where T : MonoBehaviour
{
    public static List<T> Instances { get; private set; } = new List<T>();
    int instanceIndex = 0;

    protected virtual void OnEnable()
    {
        instanceIndex = Instances.Count;
        Instances.Add(this as T);
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

public class DictTracker : MonoBehaviour
{
    public string wordPart;
    public static Dictionary<string, DictTracker> InstDict { get; private set; } = new Dictionary<string, DictTracker>();

    protected virtual void OnEnable()
    {
        InstDict.Add(wordPart, this);
    }

}
public class DictParentTracker<T> : MonoBehaviour where T : MonoBehaviour
{
    public static Dictionary<Transform, T> Instances { get; private set; } = new Dictionary<Transform, T>();

    protected virtual void OnEnable()
    {
        Instances.Add(transform.parent, this as T);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public string objectName;
    public bool working = true;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
    }

    public void FixObject()
    {
        working = true;
    }

    public void BreakObject()
    {
        IssueManager.Instance.CreateIssue(this); //Creating a new Issue because this component Broke
        working = false;
    }

    public bool IsBroken()
    {
        return !working;
    }
}

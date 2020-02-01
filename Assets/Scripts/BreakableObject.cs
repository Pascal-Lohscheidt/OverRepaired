using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public string objectName;
    public bool working = true;
    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
        renderer = GetComponent<Renderer>();
        FixObject();
    }

    public void FixObject()
    {
        renderer.material.color = Color.green;
        working = true;
    }

    public void BreakObject()
    {
        renderer.material.color = Color.red;
        IssueManager.Instance.CreateIssue(this); //Creating a new Issue because this component Broke
        working = false;
    }

    public bool IsBroken()
    {
        return !working;
    }
}

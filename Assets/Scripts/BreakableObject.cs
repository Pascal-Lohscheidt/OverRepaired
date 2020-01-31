using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    bool working = true;

    // Start is called before the first frame update
    void Start()
    {
        InGameEventManager.Instance.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleBehaviour()
    {

    }

    public void Fix()
    {
        this.working = true;
    }

    public void Break()
    {
        this.working = false;
    }

    public bool IsBroken()
    {
        return !working;
    }
}

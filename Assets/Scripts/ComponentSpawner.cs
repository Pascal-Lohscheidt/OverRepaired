using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    public List<RepairComponent> AllRepairComponents;
    // Start is called before the first frame update
    void Start()
    {
        IssueManager.Instance.OnIssueCreatetd += Instance_OnIssueCreatetd;
    }

    private void Instance_OnIssueCreatetd(Issue arg0)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

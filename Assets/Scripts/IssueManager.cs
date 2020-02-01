using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IssueManager : Singleton<IssueManager>
{
    public Dictionary<BreakableObject, Issue> currentIssueList;
    public event UnityAction<Issue, List<RepairComponent>> OnIssueCreatetd;
    public event UnityAction<Issue, BreakableObject> OnIssueFixed;
    public WordGenerator WordGen;
    public List<RepairComponent> AllRepairComponents;


    private void Start()
    {
        currentIssueList = new Dictionary<BreakableObject, Issue>();
    }

    public Issue CreateIssue(BreakableObject relatedObject)
    {
        Issue newIssue = new Issue(WordGen.generateWord(), relatedObject);
        OnIssueCreatetd(newIssue, AllRepairComponents);
        currentIssueList.Add(relatedObject,newIssue);
        WordGen.generateWord();
        return newIssue;
    }

    internal void IssueFixed(BreakableObject breakableObject)
    {
        OnIssueFixed(currentIssueList[breakableObject], breakableObject);
    }
}

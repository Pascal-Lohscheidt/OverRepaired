using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IssueManager : Singleton<IssueManager>
{
    public Dictionary<BreakableObject, Issue> currentIssueList;
    public event UnityAction<Issue, List<RepairComponent>> OnIssueCreatetd;
    public event UnityAction<Issue, BreakableObject> OnIssueFixed;
    public event UnityAction<List<RepairComponent>, List<RepairComponent>> OnWrongCreation;
    public WordGenerator WordGen;
    public List<RepairComponent> AllRepairComponentPrefaps;

    private void Start()
    {
        currentIssueList = new Dictionary<BreakableObject, Issue>();
    }

    public Issue CreateIssue(BreakableObject relatedObject)
    {
        Issue newIssue = new Issue(WordGen.generateWord(), relatedObject);
        OnIssueCreatetd(newIssue, AllRepairComponentPrefaps);
        currentIssueList.Add(relatedObject,newIssue);
        WordGen.generateWord();
        return newIssue;
    }

    internal void IssueFixed(BreakableObject breakableObject)
    {
        OnIssueFixed(currentIssueList[breakableObject], breakableObject);
    }

    internal void CaseCompindingNotNeeded(List<RepairComponent> addedComponents)
    {
        var Missing = RepairComponent.Instances.Intersect(addedComponents).ToList();
        OnWrongCreation(Missing, AllRepairComponentPrefaps);
    }
}

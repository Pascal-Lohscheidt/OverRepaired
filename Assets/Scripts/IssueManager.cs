using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IssueManager : Singleton<IssueManager>
{
    public List<Issue> currentIssueList;
    public event UnityAction<Issue, List<RepairComponent>> OnIssueCreatetd;
    public WordGenerator WordGen;
    public List<RepairComponent> AllRepairComponents;


    private void Start()
    {
        currentIssueList = new List<Issue>();
    }

    public void CreateIssue(BreakableObject relatedObject)
    {
        Issue newIssue = new Issue(WordGen.generateWord(), relatedObject);
        OnIssueCreatetd(newIssue, AllRepairComponents);
        currentIssueList.Add(newIssue);
        WordGen.generateWord();
    }

}

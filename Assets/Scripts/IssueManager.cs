using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueManager : Singleton<IssueManager>
{
    public List<Issue> currentIssueList = new List<Issue>();

    private void Start()
    {
        
    }

    public void CreateIssue(BreakableObject relatedObject)
    {
        Issue newIssue = new Issue
        {
            relatedObject = relatedObject
        };

        currentIssueList.Add(newIssue);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IssueManager : Singleton<IssueManager>
{
    public List<Issue> currentIssueList;
    public event UnityAction<Issue> OnIssueCreatetd;
    public WordGenerator WordGen;

    private void Start()
    {

    }

    public void CreateIssue(BreakableObject relatedObject)
    {
        Issue newIssue = new Issue(WordGen.generateWord(), relatedObject);
        OnIssueCreatetd(newIssue);
        currentIssueList.Add(newIssue);
        WordGen.generateWord();
    }

}

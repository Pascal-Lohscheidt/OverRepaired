using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class IssueUIHandler : Singleton<IssueUIHandler>
{
    private Dictionary<Issue, IssueTicketElement> issuesOnTheList = new Dictionary<Issue, IssueTicketElement>();
    [SerializeField] private Transform ticketListPanel; // Notification Pop down
    [SerializeField] private GameObject issueTicketElementPrefab;
    public Text MainText;
    public float ShakeAnimationDuration = 0.5f;
    public float TargetScale = 2f;
    Tween CurrentAnimation;

    private void Start()
    {
        IssueManager.Instance.OnIssueCreatetd += Instance_OnIssueCreatetd;
        IssueManager.Instance.OnIssueFixed += Instance_OnIssueFixed;
        MainText.transform.localScale = Vector3.zero;
        MainText.gameObject.SetActive(true);
    }
    public string SetFixedText(Issue Issue)
    {
        return "New issue \n" +
        Issue.relatedObject.objectName + " is broken \n" +
        Issue.seekedWord.ToString() + " is needed";
    }
    public string SetNewIssueText(Issue Issue)
    {
        return Issue.relatedObject.objectName + " has been fixed";
    }

    private void Instance_OnIssueFixed(Issue Issue, BreakableObject arg1)
    {
        Destroy(issuesOnTheList[Issue]);
        FixedTextAnimation(Issue);
    }

    private void FixedTextAnimation(Issue Needings)
    {
        MainText.text = SetFixedText(Needings);
        MainText.gameObject.SetActive(true);
        CurrentAnimation = MainText.transform.DOShakeRotation(ShakeAnimationDuration).OnComplete(CheckIfRunningOtherwiseDisable);
    }

    private void NeueIssueTextAnimation(Issue Needings)
    {
        MainText.text = SetNewIssueText(Needings);
        MainText.gameObject.SetActive(true);
        // ScaleUP
        MainText.transform.DOScale(TargetScale, ShakeAnimationDuration);
        // Rotation
        CurrentAnimation = MainText.transform.DOShakeRotation(ShakeAnimationDuration).OnComplete(CheckIfRunningOtherwiseDisable);
    }

    void CheckIfRunningOtherwiseDisable()
    {
        if (CurrentAnimation != null)
        {
            // ScaleDown
            MainText.transform.DOScale(0f, ShakeAnimationDuration).OnComplete(
                ()=> MainText.gameObject.SetActive(false));
        }
    }


    /// <summary>
    /// CreateIssueTicketElement
    /// </summary>
    /// <param name="Issue"></param>
    /// <param name="arg1"></param>
    private void Instance_OnIssueCreatetd(Issue Issue, List<RepairComponent> arg1)
    {
        IssueTicketElement instance = Instantiate(issueTicketElementPrefab, ticketListPanel).GetComponent<IssueTicketElement>();

        instance.UpdateText(Issue);

        issuesOnTheList.Add(Issue, instance);
        NeueIssueTextAnimation(Issue);
    }

}
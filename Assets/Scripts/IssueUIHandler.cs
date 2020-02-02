using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public class IssueUIHandler : Singleton<IssueUIHandler>
{
    private Dictionary<Issue, IssueTicketElement> issuesOnTheList = new Dictionary<Issue, IssueTicketElement>();
    [SerializeField] private Transform ticketListPanel; // Notification Pop down
    [SerializeField] private GameObject issueTicketElementPrefab;
    public TextMeshProUGUI MainText;
    public float ShakeAnimationDuration = 0.5f;
    public float TargetScale = 2f;
    Tween CurrentAnimation;
    AudioSource audio;

    private void Start()
    {
        audio = MainText.GetComponent<AudioSource>();
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
        Destroy(issuesOnTheList[Issue].gameObject);
        FixedTextAnimation(Issue);
    }

    private void FixedTextAnimation(Issue Needings)
    {
        MainText.text = SetNewIssueText(Needings);
        MainText.gameObject.SetActive(true);
        // ScaleUP
        CurrentAnimation= MainText.transform.DOScale(TargetScale, ShakeAnimationDuration).OnComplete(CheckIfRunningOtherwiseDisable);
        audio.PlayDelayed(0.3f);
    }

    private void NeueIssueTextAnimation(Issue Needings)
    {
        MainText.text = SetFixedText(Needings);
        MainText.gameObject.SetActive(true);
        audio.PlayDelayed(0.3f);
        // ScaleUP
        CurrentAnimation = MainText.transform.DOScale(TargetScale, ShakeAnimationDuration).OnComplete(CheckIfRunningOtherwiseDisable);
        // Rotation
        //CurrentAnimation = MainText.transform.DOShakeRotation(ShakeAnimationDuration).OnComplete(CheckIfRunningOtherwiseDisable);
    }

    void CheckIfRunningOtherwiseDisable()
    {
        if (CurrentAnimation != null)
        {
            // ScaleDown
            Sequence mySequence = DOTween.Sequence();
            mySequence.PrependInterval(3);
            var roll = MainText.transform.DOScale(0f, ShakeAnimationDuration).OnComplete(
                ()=> MainText.gameObject.SetActive(false));
            mySequence.Append(roll);
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
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class IssueUIHandler : MonoBehaviour
{
    private List<IssueTicketElement> issuesOnTheList;
    [SerializeField] private Transform ticketListPanel;
    [SerializeField] private GameObject issueTicketElementPrefab;


    public void CreateIssueTicketElement()
    {
        issuesOnTheList.Add(Instantiate(issueTicketElementPrefab, ticketListPanel).AddComponent<IssueTicketElement>());
    }

}

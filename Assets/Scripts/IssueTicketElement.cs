using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IssueTicketElement : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI breakableObjectName;
    [SerializeField] private TextMeshProUGUI componentName;

    public void UpdateText(Issue issue)
    {
        breakableObjectName.text = issue.relatedObject.objectName;
        componentName.text = issue.GetNameOfComponent();
    }
}

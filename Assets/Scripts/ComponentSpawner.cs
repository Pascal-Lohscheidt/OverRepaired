using System.Collections.Generic;
using UnityEngine;


public class ComponentSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> SpawnTransforms;
    void Start()
    {
        SpawnTransforms = SetChilds();
        IssueManager.Instance.OnIssueCreatetd += Instance_OnIssueCreatetd;
        IssueManager.Instance.OnWrongCreation += CreatMissingReapirComponents;
    }

    private void CreatMissingReapirComponents(List<RepairComponent> missingObj, List<RepairComponent> RepariComps)
    {
        var _spownTransform = SpawnTransforms[Random.Range(0, SpawnTransforms.Count)];
        foreach (var prefap in RepariComps)
            foreach (var item in missingObj)
                if (item.partName == prefap.partName)
                    Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity);
    }

    private List<Transform> SetChilds()
    {
        var childs = new List<Transform>();
        foreach (Transform child in transform)
            childs.Add(child);
        return childs;
    }

    private void Instance_OnIssueCreatetd(Issue IssueEvent, List<RepairComponent> RepariComps)
    {

        foreach (RepairComponent item in RepariComps)
        {
            var _spownTransform = SpawnTransforms[Random.Range(0, SpawnTransforms.Count)];
            if(item.partName == IssueEvent.seekedWord.Prefix)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity));
            }
            if (item.partName == IssueEvent.seekedWord.BaseWord)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity));
            }
            if (item.partName == IssueEvent.seekedWord.Suffix)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity));
            }

        }
    }


    public void SetColor(RepairComponent instanz)
    {
        try
        {
            instanz.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
        }
        catch (System.Exception)
        {
            instanz.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}

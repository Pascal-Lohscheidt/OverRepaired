using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> SpawnTransforms;
    void Start()
    {
        IssueManager.Instance.OnIssueCreatetd += Instance_OnIssueCreatetd;
    }

    private void Instance_OnIssueCreatetd(Issue IssueEvent, List<RepairComponent> RepariComps)
    {
        
        foreach (RepairComponent item in RepariComps)
        {
            var _spownTransform = SpawnTransforms[Random.Range(0, SpawnTransforms.Count)];
            if(item.PartName == IssueEvent.SeekedWord.Prefix)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity));
            }
            if (item.PartName == IssueEvent.SeekedWord.BaseWord)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.position, Quaternion.identity));
            }
            if (item.PartName == IssueEvent.SeekedWord.Suffix)
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ComponentSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public List<SpownPointSpot> SpawnTransforms;
    void Start()
    {
        SpawnTransforms = GetComponentsInChildren<SpownPointSpot>().ToList();
        IssueManager.Instance.OnIssueCreatetd += Instance_OnIssueCreatetd;
        IssueManager.Instance.OnWrongCreation += CreatMissingReapirComponents;
    }

    private void CreatMissingReapirComponents(List<RepairComponent> missingObj, List<RepairComponent> RepariComps)
    {
        var ListOfPossibleSpownSpots = SpawnTransforms.Where(x => x.isSpawnSpotBussy).ToList();
        foreach (var prefap in RepariComps)
            foreach (var item in missingObj)
                if (item.partName == prefap.partName)
                {
                    
                    var _spownTransform = ListOfPossibleSpownSpots[Random.Range(0, SpawnTransforms.Count)];
                    Instantiate<RepairComponent>(item, _spownTransform.transform.position, Quaternion.identity);
                }
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
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.transform.position, Quaternion.identity));
            }
            if (item.partName == IssueEvent.seekedWord.BaseWord)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.transform.position, Quaternion.identity));
            }
            if (item.partName == IssueEvent.seekedWord.Suffix)
            {
                SetColor(Instantiate<RepairComponent>(item, _spownTransform.transform.position, Quaternion.identity));
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

//[SerializeField]
//public struct SpownPoint
//{
//    Transform SpawnTransformsSpownPoint;

//}

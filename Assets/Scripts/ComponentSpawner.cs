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
        var ListOfPossibleSpownSpots = SpawnTransforms.Where(x => !x.isSpawnSpotBussy).ToList();
        foreach (var prefap in RepariComps)
            foreach (var item in missingObj)
                if (item.partName == prefap.partName)
                    CreateObject(item, ListOfPossibleSpownSpots);
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
        var ListOfPossibleSpownSpots = SpawnTransforms.Where(x => !x.isSpawnSpotBussy).ToList();
        foreach (RepairComponent item in RepariComps)
        {
            if(item.partName == IssueEvent.seekedWord.Prefix)
            {
                CreateObject(item, ListOfPossibleSpownSpots);
            }
            if (item.partName == IssueEvent.seekedWord.BaseWord)
            {
                CreateObject(item, ListOfPossibleSpownSpots);
            }
            if (item.partName == IssueEvent.seekedWord.Suffix)
            {
                CreateObject(item, ListOfPossibleSpownSpots);
            }
        }
    }

    private RepairComponent CreateObject(RepairComponent item, List<SpownPointSpot> listOfPossibleSpownSpots)
    {
        if (listOfPossibleSpownSpots.Count == 0)
            return null;
        int index = Random.Range(0, listOfPossibleSpownSpots.Count - 1);
        var _spownTransform = listOfPossibleSpownSpots[index];
        listOfPossibleSpownSpots.RemoveAt(index);
        var instanz = Instantiate<RepairComponent>(item, _spownTransform.transform.position, Quaternion.identity);
        SetColor(instanz);
        return instanz;
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

using UnityEngine;
using System.Collections.Generic;

public class WordGenerator : MonoBehaviour
{
    public List<RepairComponent> baseWord = new List<RepairComponent>();

    private void Start()
    {
        //baseWord = GetBaseWords(IssueManager.Instance.AllRepairComponentPrefaps);
    }

    //private List<string> GetBaseWords(List<RepairComponent> allRepairComponents)
    //{
    //    baseWord = new List<string>();
    //    foreach (RepairComponent item in allRepairComponents)
    //        baseWord.Add(item.partName);
    //    return baseWord;
    //}

    public SerchedComponentenWort generateWord()
    {
        List<RepairComponent> preList = IssueManager.Instance.AllRepairComponentPrefaps.FindAll(i => i.componentType == RepairComponent.ComponentType.Bottom);
        List<RepairComponent> baseList = IssueManager.Instance.AllRepairComponentPrefaps.FindAll(i => i.componentType == RepairComponent.ComponentType.Middle);
        List<RepairComponent> suffixList = IssueManager.Instance.AllRepairComponentPrefaps.FindAll(i => i.componentType == RepairComponent.ComponentType.Top);

        return new SerchedComponentenWort
        {
           
            Prefix = preList[Random.Range(0, preList.Count)].partName,
            BaseWord = baseList[Random.Range(0, baseList.Count)].partName,
            Suffix = suffixList[Random.Range(0, suffixList.Count)].partName
        };
    }
}
public struct SerchedComponentenWort
{
    public string Prefix;
    public string BaseWord;
    public string Suffix;

    public override string ToString()
    {
        return Prefix + BaseWord + Suffix;
    }
}
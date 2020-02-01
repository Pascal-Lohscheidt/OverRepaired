using UnityEngine;
using System.Collections.Generic;

public class WordGenerator : MonoBehaviour
{
    public List<string> BaseWord;

    private void Start()
    {
        BaseWord = GetBaseWords(IssueManager.Instance.AllRepairComponentPrefaps);
    }

    private List<string> GetBaseWords(List<RepairComponent> allRepairComponents)
    {
        BaseWord = new List<string>();
        foreach (RepairComponent item in allRepairComponents)
            BaseWord.Add(item.partName);
        return BaseWord;
    }

    public SerchedComponentenWort generateWord()
    {
        return new SerchedComponentenWort()
        {
        Prefix = BaseWord[Random.Range(0, BaseWord.Count)],
        BaseWord = BaseWord[Random.Range(0, BaseWord.Count)],
        Suffix = BaseWord[Random.Range(0, BaseWord.Count)]
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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Issue
{
    public BreakableObject relatedObject;
    public List<RepairComponent> requiredComponents;
    public SerchedComponentenWort seekedWord;

    public Issue(SerchedComponentenWort seekedWords, BreakableObject relatedObject)
    {
        this.relatedObject = relatedObject;
        this.seekedWord = seekedWords; 
    }

    public string GetNameOfComponent()
    {
        return seekedWord.ToString();
    }

    public bool ComponentsMatchIssue(List<RepairComponent> compareList)
    {
        foreach (RepairComponent comp in requiredComponents)
            if (!compareList.Exists(i => i.partName == comp.partName)) return false;
        return true;
    }

    public string ReturnProperName()
    {
        return GetNameOfComponent();
    }
}

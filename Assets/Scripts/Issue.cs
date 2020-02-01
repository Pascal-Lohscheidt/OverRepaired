using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Issue
{
    public BreakableObject relatedObject;
    public List<RepairComponent> requiredComponents;
    public SerchedComponentenWort SeekedWord;

    public Issue(SerchedComponentenWort SeekedWord, BreakableObject relatedObject)
    {
        this.relatedObject = relatedObject;
        this.SeekedWord = SeekedWord; 
    }

    public string GetNameOfComponent()
    {
        string newName = "";

        foreach (RepairComponent comp in requiredComponents) newName += comp.name;

        return newName;
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

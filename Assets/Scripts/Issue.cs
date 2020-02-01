using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Issue
{
    public BreakableObject relatedObject;
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
        List<string> words = new List<string>();
        words.Add(seekedWord.Prefix);
        words.Add(seekedWord.BaseWord);
        words.Add(seekedWord.Suffix);

        foreach (string word in words)
            if (!compareList.Exists(i => i.partName == word)) return false;
        return true;
    }

    public string ReturnProperName()
    {
        return GetNameOfComponent();
    }
}

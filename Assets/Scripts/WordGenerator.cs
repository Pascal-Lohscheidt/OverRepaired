using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordGenerator : MonoBehaviour
{
    public List<string> Prefix;
    public List<string> BaseWord;
    public List<string> Suffix;

    public SerchedComponentenWort generateWord()
    {
        return new SerchedComponentenWort()
        {
        Prefix = Prefix[Random.Range(0, Prefix.Count)],
        BaseWord = BaseWord[Random.Range(0, BaseWord.Count)],
        Suffix = Suffix[Random.Range(0, Suffix.Count)]
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
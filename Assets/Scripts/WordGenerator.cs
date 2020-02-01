using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordGenerator : MonoBehaviour
{
    public List<string> BaseWord;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : Singleton<AudioManager>
{
    public List<AudioClip> AudioClips;
    public Dictionary<string, AudioClip> NamedClips = new Dictionary<string, AudioClip>();
    AudioSource Audio;
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        NamedClips = AudioClips.ToDictionary(k => k.name, A => A);
    }

    public void PlaySound(string ClipName, float Delay = 0f)
    {
        Audio.clip = NamedClips[ClipName];
        Audio.PlayDelayed(Delay);
    }
}

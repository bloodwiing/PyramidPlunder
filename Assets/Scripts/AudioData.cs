using UnityEngine;
using System;

[Serializable]
public class AudioData
{
    public string ID;

    [Range(0f, 1f)]
    public float volume;
    public bool loop;
    [Range(-3f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
    public AudioClip clip;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public AudioData[] registeredSFX;

    void Awake()
    {
        foreach (var sfx in registeredSFX)
        {
            sfx.source = gameObject.AddComponent<AudioSource>();
            sfx.source.playOnAwake = false;
            sfx.source.clip = sfx.clip;
            sfx.source.volume = sfx.volume;
            sfx.source.loop = sfx.loop;
            sfx.source.pitch = sfx.pitch;
        }
    }

    public void Stop(string ID)
    {
        registeredSFX.Where(sfx => sfx.ID == ID).ToArray()[0].source.Stop();
    }

    public void Play(string ID)
    {
        registeredSFX.Where(sfx => sfx.ID == ID).ToArray()[0].source.Play();
    }
}

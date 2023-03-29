using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource BGMPlayer;
    public AudioSource SEPlayer;

    public void SetBGMVolume(float volume)
    {
        BGMPlayer.volume = volume;
    }

    public void SetSEVolume(float volume)
    {
        SEPlayer.volume = volume;
    }

    public float GetBGMVolume() => BGMPlayer.volume;
    public float GetSEVolume() => SEPlayer.volume;
}

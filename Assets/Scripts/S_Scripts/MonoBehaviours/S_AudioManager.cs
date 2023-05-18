using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AudioManager : MonoBehaviour
{
    public S_CentralAccessor accessor;

    [Header("AudioSource")]
    public AudioSource BGMPlayer;
    public AudioSource SEPlayer;

    public void SetBGMVolume(float volume)
    {
        BGMPlayer.volume = volume;
        accessor.ProcessManager.SaveProfile();
    }

    public void SetSEVolume(float volume)
    {
        SEPlayer.volume = volume;
        accessor.ProcessManager.SaveProfile();
    }

    public float GetBGMVolume() => BGMPlayer.volume;
    public float GetSEVolume() => SEPlayer.volume;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_SettingPanelManager : MonoBehaviour
{
    public Slider BGMSlider;
    public Slider SESlider;

    public S_CentralAccessor Accessor;

    //private void Start()
    //{
    //    BGMSlider.value = Accessor.AudioManager.GetBGMVolume();
    //    SESlider.value = Accessor.AudioManager.GetSEVolume();
    //}

    private void OnEnable()
    {
        Accessor.ProcessManager.LoadProfile();
        
    }
}

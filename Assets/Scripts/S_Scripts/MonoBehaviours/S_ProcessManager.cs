using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ProcessManager : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}

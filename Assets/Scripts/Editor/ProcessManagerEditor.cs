using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(S_ProcessManager))]
public class ProcessManagerEditor : Editor
{
    private S_ProcessManager m_target;
    private void OnEnable()
    {
        m_target = target as S_ProcessManager; //∞Û∂®target£¨targetπŸ∑ΩΩ‚ Õ£∫ The object being inspected
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DeleteSave"))
        {
            m_target.DeleteSaving();
        }

        if (GUILayout.Button("LoadSave"))
        {
            m_target.Load();
        }

        GUILayout.EndHorizontal();
    }
}

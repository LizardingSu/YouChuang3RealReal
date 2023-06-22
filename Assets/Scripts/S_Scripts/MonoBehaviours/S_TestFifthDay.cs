using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class S_TestFifthDay : MonoBehaviour
{
    public ScriptableObject testSO;

    public void StartFromFifthDay()
    {
        S_ProcessManager p = GameObject.Find("MainManager").GetComponent<S_ProcessManager>();
        p.WriteFile(testSO, p.m_SavingName1);
        Debug.Log("存档更新成功");
        p.GetComponent<S_CentralAccessor>().MenuManager.ButtonsTransform.GetChild(1).GetComponent<Button>().interactable = true;
    }
}

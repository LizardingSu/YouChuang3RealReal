using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class LogEntryController : MonoBehaviour
{
    public LogEntry m_logEntry;

    //text
    public TMP_Text m_name;
    public TMP_Text m_dio;
    public SelectController m_selectController;

    public Image m_dioBox;
    public VerticalLayoutGroup m_layoutGroup;
    public VerticalLayoutGroup c_layoutGroup;

    public RectOffset m_unselectPadding;
    //肯定是右边，因为只有主角能出选项
    public RectOffset m_selectPadding;

    public void Init(LogEntry logEntry,bool isFirstTime)
    {

        bool isReversed = !logEntry.Left;
        bool isSelected = logEntry.Select;

        m_name.text = logEntry.Name;

        if (isSelected)
        {
            c_layoutGroup.padding = m_selectPadding;

            m_dio.gameObject.SetActive(false);
            m_selectController.gameObject.SetActive(true);
            m_selectController.Init(logEntry,false);
        }
        else
        {
            c_layoutGroup.padding = m_unselectPadding;

            m_dio.gameObject.SetActive(true);
            m_dio.text = logEntry.Log;

            m_selectController.gameObject.SetActive(false);
        }

        SetLogReverse(isReversed, isSelected);
    }

    private void SetLogReverse(bool isReverse,bool isSelected)
    {
        Vector3 scale = isReverse ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

        //Control Log Left or Right
        if (!isReverse)
        {
            m_layoutGroup.childAlignment = TextAnchor.UpperLeft;
            m_layoutGroup.padding.left = -24;
            c_layoutGroup.childAlignment = TextAnchor.UpperLeft;

        }
        else
        {
            m_layoutGroup.childAlignment = TextAnchor.UpperRight;
            m_layoutGroup.padding.left = 52;
            c_layoutGroup.childAlignment = TextAnchor.UpperRight;
        }

        m_dioBox.transform.localScale = scale;
        foreach (Transform child in m_dioBox.transform)
        {
            child.localScale = scale;
        }
    }

}

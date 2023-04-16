using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightLogController : MonoBehaviour
{
    public TMP_Text m_dio;
    public SelectController m_select;

    public void Init(LogEntry logEntry)
    {
        bool isSelected = logEntry.Select;
        if (isSelected)
        {
            m_dio.gameObject.SetActive(false);
            m_select.gameObject.SetActive(true);

            m_select.Init(logEntry, true);
        }
        else
        {
            m_dio.gameObject.SetActive(true);
            m_select.gameObject.SetActive(false);

            m_dio.text = logEntry.Log;
        }
    }
}

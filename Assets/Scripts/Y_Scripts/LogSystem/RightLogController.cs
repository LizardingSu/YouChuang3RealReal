using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightLogController : MonoBehaviour
{
    public RectTransform m_transform;
    public TMP_Text m_dio;
    public SelectController m_select;

    public List<Image> buttonImage;

    public void Awake()
    {
        foreach(var image in buttonImage)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void MoveUp(float time)
    {
        if (m_transform.anchoredPosition.y == 0) return;
        m_transform.DOAnchorPosY(0,time);
    }
    public void MoveDown(float time)
    {
        if (m_transform.anchoredPosition.y == -400) return;
        m_transform.DOAnchorPosY(-400, time);
    }

    public void Init(LogEntry logEntry)
    {
        bool isSelected = logEntry.Select;
        if (isSelected)
        {
            m_dio.gameObject.SetActive(false);
            m_select.gameObject.SetActive(true);

            m_select.Init(logEntry, true, buttonImage);
        }
        else
        {
            m_dio.gameObject.SetActive(true);
            m_select.gameObject.SetActive(false);

            foreach (var image in buttonImage)
            {
                image.gameObject.SetActive(false);
            }

            m_dio.text = logEntry.Log;
        }
    }
}

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

    public DioLogueState diostate;

    public List<Image> buttonImage;

    public float speed = 15.0f;

    Coroutine coroutine;

    public void Awake()
    {
        foreach (var image in buttonImage)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void MoveUp(float time)
    {
        //not really need
        m_transform.DOKill(true);

        if (m_transform.anchoredPosition.y == 0) return;

        m_transform.DOAnchorPosY(0, time);
    }
    public void MoveDown(float time)
    {

        //not really need
        m_transform.DOKill(true);

        if (m_transform.anchoredPosition.y == -400) return;

        m_transform.DOAnchorPosY(-400, time);
    }


    public void KillAllAnim()
    {
        m_transform.DOKill(true);

        StopCoroutine(coroutine);
        m_dio.maxVisibleCharacters = m_dio.textInfo.characterCount;
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
            //保证每次都能正常显示所有Character。
            m_dio.maxVisibleCharacters = int.MaxValue;

            if (diostate.state == DioState.Normal)
                coroutine = StartCoroutine(TypeWriter(m_dio, speed));
        }
    }

    private IEnumerator TypeWriter(TMP_Text textComponent, float speed)
    {
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int total = textInfo.characterCount;
        bool complete = false;
        int current = 0;

        while (!complete)
        {
            if (current > total)
            {
                current = total;
                yield return new WaitForSeconds(1 / speed);
                complete = true;
            }

            textComponent.maxVisibleCharacters = current;
            current += 1;

            yield return new WaitForSeconds(1 / speed);
        }

        yield return null;
    }

}

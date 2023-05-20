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

        //not really need
        m_transform.DOKill(true);
        m_transform.DOAnchorPosY(0,time);
    }
    public void MoveDown(float time)
    {
        if (m_transform.anchoredPosition.y == -400) return;

        //not really need
        m_transform.DOKill(true);

        m_transform.DOAnchorPosY(-400, time);
    }

    public void KillAllAnim()
    {
        m_transform.DOKill(true);
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

    private IEnumerator PrintText(float speed)
    {
        TMP_TextInfo textInfo = m_dio.textInfo;

        //首先清空所有文字网格
        for (int i = 0; i < textInfo.materialCount; ++i)
        {
            textInfo.meshInfo[i].Clear();
        }
        m_dio.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        float timer = 0;
        float duration = 1 / speed;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            while (timer < duration)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            timer -= duration;

            TMPro.TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

            int materialIndex = characterInfo.materialReferenceIndex;
            int verticeIndex = characterInfo.vertexIndex;
            if (characterInfo.elementType == TMPro.TMP_TextElementType.Sprite)
            {
                verticeIndex = characterInfo.spriteIndex;
            }
            if (characterInfo.isVisible)
            {
                textInfo.meshInfo[materialIndex].vertices[0 + verticeIndex] = characterInfo.vertex_BL.position;
                textInfo.meshInfo[materialIndex].vertices[1 + verticeIndex] = characterInfo.vertex_TL.position;
                textInfo.meshInfo[materialIndex].vertices[2 + verticeIndex] = characterInfo.vertex_TR.position;
                textInfo.meshInfo[materialIndex].vertices[3 + verticeIndex] = characterInfo.vertex_BR.position;
                m_dio.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }

        }

    }
}

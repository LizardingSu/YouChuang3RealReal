using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOutAnim : MonoBehaviour
{
    public RectTransform m_chainRect1;
    public RectTransform m_chainRect2;

    public Sequence m_sequence1;
    public Sequence m_sequence2;


    void Awake()
    {
        m_sequence1 = DOTween.Sequence();
        m_sequence2 = DOTween.Sequence();

        ChainAnim();
    }

    private void ChainAnim()
    {
        m_chainRect1.anchoredPosition = Vector2.zero;
        m_chainRect2.anchoredPosition = new Vector2(-1920,0);

        //上边的Chain
        m_sequence1.Append(m_chainRect1.DOAnchorPosX(-2520, 0.8f));
        m_sequence1.AppendInterval(0.2f);

        m_sequence1.Append(m_chainRect1.DOAnchorPosX(-2300,0.15f));
        m_sequence1.AppendInterval(0.05f);
        m_sequence1.Append(m_chainRect1.DOAnchorPosX(-2520, 0.2f));

        //下边的Chain
        m_sequence2.Append(m_chainRect2.DOAnchorPosX(560, 0.8f));
        m_sequence2.AppendInterval(0.2f);

        m_sequence2.Append(m_chainRect2.DOAnchorPosX(340, 0.15f));
        m_sequence2.AppendInterval(0.05f);
        m_sequence2.Append(m_chainRect2.DOAnchorPosX(560, 0.2f));

    }

}

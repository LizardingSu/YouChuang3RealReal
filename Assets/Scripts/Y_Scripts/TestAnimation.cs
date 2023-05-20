using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Button m_button;
    private RectTransform m_rectTransform;

    public void Start()
    {
        m_button.onClick.AddListener(onClick);
        m_rectTransform = m_button.GetComponent<RectTransform>();

        m_rectTransform.DOAnchorPosX(1000, 8);
    }

    private void OnDestroy()
    {
        m_button.onClick.RemoveListener(onClick);
    }
    public void onClick()
    {
        m_rectTransform.DOKill(true);
        m_rectTransform.DOAnchorPosX(-1000, 8);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

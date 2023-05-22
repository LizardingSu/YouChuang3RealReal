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
    public Image m_image;

    Sequence s;

    public void Start()
    {
        s = DOTween.Sequence();

        m_rectTransform = m_button.GetComponent<RectTransform>();

        s.Append(m_rectTransform.DOAnchorPosX(1000, 3)).AppendCallback(() =>
        {
            Debug.Log("WOWOWOW");
        });

        s.Join(m_image.DOColor(new Color(1, 1, 0, 1), 3));

        s.Append(m_rectTransform.DOAnchorPosX(-1000, 3));
    }

    private void OnDestroy()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

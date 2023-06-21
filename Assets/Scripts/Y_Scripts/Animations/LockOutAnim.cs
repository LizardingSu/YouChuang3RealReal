using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockOutAnim : MonoBehaviour
{
    public DioLogueState state;
    public S_ProcessManager process;

    public RectTransform m_chainRect1;
    public RectTransform m_chainRect2;

    public Sequence m_sequence;

    public RectTransform blackMask;
    public RectTransform blackScene;

    public Button buttonToContinue;
    public TMP_Text dayT;
    public TMP_Text numT;
    public TMP_Text pressT;
    public TMP_Text tipsT;

    public RectTransform mask;

    public float moveUpBlackMaskTime = 1.2f;
    public float fadeInTime = 0.5f;

    public int day;


    private void Awake()
    {
        buttonToContinue.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        buttonToContinue.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        state.SetButtonsActive(false);
        state.ReadToCurrentID((int)(day-3), -1);

        WaitTime(2);
    }

    private void WaitTime(float time)
    {
        m_sequence = DOTween.Sequence();

        blackMask.gameObject.SetActive(false);
        mask.gameObject.SetActive(true);

        m_sequence.Append(numT.DOColor(new Color(1,1,1,0),1));
        m_sequence.Join(dayT.DOColor(new Color(1, 1, 1, 0), 1));
        m_sequence.Join(pressT.DOColor(new Color(0.8f, 0.8f, 0.8f, 0), 1));
        m_sequence.Join(tipsT.DOColor(new Color(1, 1, 0, 0), 1));
        m_sequence.Join(blackScene.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 1));
        m_sequence.Join(m_chainRect1.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 1));
        m_sequence.Join(m_chainRect2.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 1));

        m_sequence.AppendInterval(0.5f).AppendCallback(() =>
        {
            blackScene.gameObject.SetActive(false);

            state.ProcessInput();
            process.Save((int)(day-3) * 1000, 1, "");
        });

        m_sequence.AppendInterval(0.5f).AppendCallback(() =>
        {
            mask.gameObject.SetActive(false);
            state.SetButtonsActive(true);
        });
    }

    public void LockOutScene(int day,bool lockOut = true)
    {
        m_sequence = DOTween.Sequence();

        //初始化状态
        state.SetButtonsActive(false);
        buttonToContinue.interactable = false;

        this.day = day;
        this.numT.text = day.ToString();

        blackMask.gameObject.SetActive(true);
        blackMask.anchoredPosition = new Vector2(0, 440.0f);
        blackScene.gameObject.SetActive(false);
        blackScene.GetComponent<Image>().color = new Color(0, 0, 0, 1);

        m_chainRect1.anchoredPosition = Vector2.zero;
        m_chainRect2.anchoredPosition = new Vector2(-1920, 0);
        m_chainRect1.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        m_chainRect2.GetComponent<Image>().color = new Color(1, 1, 1, 1);



        if (!lockOut && day == 4)
            tipsT.text = "恭喜你通过了这四天！第五天还没做。";
        else if (!lockOut && day == 8)
            tipsT.text = "恭喜你通过了这八天！第九天还没做。";
        else
            tipsT.text = "在过去四天里好像还有可以深入的话题，还请再考虑一下。";

            var inv = new Color(1, 1, 1, 0);
        var v = new Color(1, 1, 1, 1);

        m_sequence.Append(blackMask.DOAnchorPos3DY(2160.0f, moveUpBlackMaskTime)).AppendCallback(() =>
        {
            blackScene.gameObject.SetActive(true);
            pressT.gameObject.SetActive(false);
            tipsT.gameObject.SetActive(false);
            numT.rectTransform.anchoredPosition = Vector2.zero;

            //淡入淡出
            dayT.color = inv;
            numT.color = inv;
            pressT.color = inv;
            tipsT.color = inv;
        });

        m_sequence.Append(dayT.DOColor(v, fadeInTime));
        m_sequence.Join(numT.DOColor(v, fadeInTime)).AppendCallback(() =>
        {
            tipsT.gameObject.SetActive(true);
            pressT.gameObject.SetActive(true);
        });

        m_sequence.Append(numT.rectTransform.DOAnchorPosY(70, 1.0f));

        //上边的Chain
        m_sequence.Append(m_chainRect1.DOAnchorPosX(-2520, 0.8f));
        m_sequence.Join(m_chainRect2.DOAnchorPosX(560, 0.8f));
        m_sequence.Join(numT.rectTransform.DOAnchorPosY(-40, 0.6f));

        m_sequence.AppendInterval(0.2f);

        m_sequence.Append(m_chainRect1.DOAnchorPosX(-2300, 0.15f));
        m_sequence.Join(m_chainRect2.DOAnchorPosX(340, 0.15f));

        m_sequence.AppendInterval(0.05f);
        m_sequence.Join(numT.rectTransform.DOAnchorPosY(0, 0.3f));

        m_sequence.Append(m_chainRect1.DOAnchorPosX(-2520, 0.2f));
        m_sequence.Join(m_chainRect2.DOAnchorPosX(560, 0.2f));

        m_sequence.Append(tipsT.DOColor(new Color(1,1,0,1), fadeInTime + 1));
        m_sequence.Join(pressT.DOColor(new Color(0.8f,0.8f,0.8f,1), fadeInTime)).AppendCallback(() =>
        {
            buttonToContinue.interactable = true;
        });

    }

}

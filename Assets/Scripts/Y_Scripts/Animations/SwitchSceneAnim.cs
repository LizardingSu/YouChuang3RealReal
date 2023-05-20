using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSceneAnim : MonoBehaviour
{
    public float downPos = 440.0f;
    public float daysPos = 200.0f;

    public float moveUpBlackMaskTime = 1.2f;
    public float delayToTurnTime = 0.2f;
    public float turningTime = 1.2f;

    public uint nextDay;

    public Image blackMask;
    public RectTransform blackScene;

    public TMP_Text pressToContinue;

    public RectTransform daysTransform;
    public TMP_Text day;
    public TMP_Text day_First;
    public TMP_Text day_Second;

    public DioLogueState diologueState;
    public S_ProcessManager processManager;
    public Button buttonToContinue;

    public Selectable mask;

    private void MoveUpBlackMask(float time)
    {
        blackMask.gameObject.SetActive(true);
        blackMask.rectTransform.anchoredPosition = new Vector2(0, downPos);

        blackMask.rectTransform.DOKill(true);
        blackMask.rectTransform.DOAnchorPos3DY(2160.0f, time);
    }

    private IEnumerator DaysTurn(float delay1,float delay2,float time,uint day1,uint day2)
    {
        //为了防止什么莫名其妙的情况
        blackScene.gameObject.SetActive(false);
        //先等待黑色幕布移动上来
        yield return new WaitForSeconds(delay1);
        //将blackScene设置为Active
        blackScene.gameObject.SetActive(true);
        //将各项都初始化
        if(day1 == 0)
            day_First.text = "?";
        else
            day_First.text = Convert.ToString(day1);
        day_Second.text = Convert.ToString(day2);
        daysTransform.anchoredPosition = new Vector2(daysTransform.anchoredPosition.x, 0);
        pressToContinue.gameObject.SetActive(false);

        var inv = new Color(1, 1, 1, 0);
        var v = new Color(1, 1, 1, 1);

        //淡入淡出
        day.color = inv;
        day_First.color = inv;
        day_Second.color = inv;
        pressToContinue.color = inv;
        day.DOColor(v, delay2 + 2);
        day_First.DOColor(v, delay2 + 2);
        day_Second.DOColor(v, delay2 + 2);

        yield return new WaitForSeconds(delay2);

        daysTransform.DOAnchorPosY(daysPos, time);

        StartCoroutine(setPressActive(time));
    }

    private IEnumerator setPressActive(float time)
    {
        pressToContinue.gameObject.SetActive(true);
        pressToContinue.DOColor(Color.white, time+2);

        yield return new WaitForSeconds(time);

        buttonToContinue.interactable = true;
    }

    private void onClick()
    {
        pressToContinue.DOKill(true);
        diologueState.SetButtonsActive(false);
        diologueState.ReadToCurrentID((int)nextDay, -1);

        StartCoroutine(WaitTime(2));
    }

    private IEnumerator WaitTime(float time)
    {
        blackMask.gameObject.SetActive(false);
        mask.gameObject.SetActive(true);

        day.DOColor(new Color(1,1,1,0), 1);
        day_First.DOColor(new Color(1, 1, 1, 0), 1);
        day_Second.DOColor(new Color(1, 1, 1, 0), 1);
        pressToContinue.DOColor(new Color(1, 1, 1, 0), 1);
        blackScene.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 1);

        yield return new WaitForSeconds(1);
        blackScene.gameObject.SetActive(false);

        yield return new WaitForSeconds(time-1);
        mask.gameObject.SetActive(false);
        diologueState.ProcessInput();
        processManager.Save((int)nextDay * 1000,1,"");
        diologueState.SetButtonsActive(true);
    }

    public void SwitchToNewScene(uint day1,uint day2)
    {
        //初始化状态
        diologueState.SetButtonsActive(false);
        buttonToContinue.interactable = false;
        nextDay = day2;

        MoveUpBlackMask(moveUpBlackMaskTime);
        StartCoroutine(DaysTurn(moveUpBlackMaskTime, delayToTurnTime, turningTime, day1, day2));
    }

    private void Awake()
    {
        buttonToContinue.onClick.AddListener(onClick);
    }

    private void OnDestroy()
    {
        buttonToContinue.onClick.RemoveListener(onClick);
    }

}

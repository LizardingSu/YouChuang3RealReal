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
    public TMP_Text day_First;
    public TMP_Text day_Second;

    public DioLogueState diologueState;
    public Button buttonToContinue;

    private void MoveUpBlackMask(float time)
    {
        blackMask.rectTransform.anchoredPosition = new Vector2(0, downPos);

        blackMask.rectTransform.DOKill();
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
        day_First.text = Convert.ToString(day1);
        day_Second.text = Convert.ToString(day2);
        daysTransform.anchoredPosition = new Vector2(daysTransform.anchoredPosition.x, 0);
        pressToContinue.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay2);
        daysTransform.DOAnchorPosY(daysPos, time);

        StartCoroutine(setPressActive(time));
    }

    private IEnumerator setPressActive(float time)
    {
        yield return new WaitForSeconds(time);

        pressToContinue.gameObject.SetActive(true);
        buttonToContinue.interactable = true;
    }

    private void onClick()
    {
        diologueState.SetButtonsActive(true);
        this.gameObject.SetActive(false);

        diologueState.ReadToCurrentID((int)nextDay, -1);
    }

    public void SwitchToNewScene(uint day1,uint day2)
    {
        //初始化状态
        this.gameObject.SetActive(true);
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

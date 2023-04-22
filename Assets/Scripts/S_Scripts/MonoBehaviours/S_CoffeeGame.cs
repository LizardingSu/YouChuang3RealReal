using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_CoffeeGame : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public GameObject GamePanel;

    public bool GamePlaying;

    public int frameNumber = 30;

    public float GamePanelOriginPosY;

    public float DeltaY;

    public GameObject ObjectScene;

    [Header("动画帧")]
    public Sprite[] LockerFrames;

    public Sprite[] RefrigeratorFrames;

    public Sprite[] GrinderFrames;


    public GameObject FinishButton;

    public GameObject RestartButton;

    public GameObject Order;

    public GameObject State;

    public GameObject Cup;

    public GameObject PaperCup;

    private void Start()
    {
        //DTList = DOTween.Sequence();
        
        //添加游戏开始事件监听
        DioLogueState ds = accessor._DioLogueState;
        ds.dialogueChanged.AddListener(StartCoffeeGame);
        
        for (int i = 0; i < ObjectScene.transform.childCount; i++)
        {
            ObjectScene.transform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        }

        GamePanelOriginPosY = GetComponent<RectTransform>().position.y;
        GamePlaying = false;
    }

    public void InitGame(int id)
    {

    }

    [Obsolete("喻洛天，记得改成新重载的带ID的改版")]
    public void StartCoffeeGame(DiologueData data)
    {
        if (data.processState == ProcessState.Coffee && accessor._DioLogueState.state == DiologueState.Normal)
        {
            //控制面板移动
            RectTransform r = GetComponent<RectTransform>();
            r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY + DeltaY), 1.6f);

            //YLT用来作不明觉厉功能的代码
            DioLogueState ds = accessor._DioLogueState;
            ds.SetButtonsActive(false);


        }
        else
        {
            return;
        }
    }

    public void StartCoffeeGame(DiologueData data, int GameID)
    {
        if (data.processState == ProcessState.Coffee && accessor._DioLogueState.state == DiologueState.Normal)
        {
            //控制面板移动
            RectTransform r = GetComponent<RectTransform>();
            r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY + DeltaY), 1.6f);

            //YLT用来作不明觉厉功能的代码
            DioLogueState ds = accessor._DioLogueState;
            ds.SetButtonsActive(false);

            InitGame(GameID);
        }
        else
        {
            return;
        }
    }

    public void EndCoffeeGame()
    {
        if (accessor._DioLogueState.state == DiologueState.Normal)
        {
            //StartCoroutine(HideGamePanel());
            RectTransform r = GetComponent<RectTransform>();
            r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY), 2.0f);
            DioLogueState ds = accessor._DioLogueState;
            ds.SetButtonsActive(true);
            //StartCoroutine(SetButtonsActive(1.2f, true));
        }

        accessor._DioLogueState.UpdateDiologue();
    }

    //[Obsolete]
    //IEnumerator ShowGamePanel()
    //{
    //    for (int i = 1; i <= frameNumber; i++)
    //    {
    //        float currentY = Mathf.Lerp(GamePanelOriginPosY, GamePanelShowedPosY, (float)i / (float)frameNumber);
    //        GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    //[Obsolete]
    //IEnumerator HideGamePanel()
    //{
    //    for (int i = 1; i <= frameNumber; i++)
    //    {
    //        float currentY = Mathf.Lerp(GamePanelShowedPosY, GamePanelOriginPosY, (float)i / (float)frameNumber);
    //        GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
    //        yield return new  WaitForFixedUpdate();
    //    }
    //}

    //IEnumerator SetButtonsActive(float delay,bool active)
    //{
    //    yield return new WaitForSeconds(delay);
    //    DioLogueState ds = accessor._DioLogueState;
    //    ds.SetButtonsActive(active);
    //}

    private void Update()
    {
        //测试
        if (Input.GetKeyDown(KeyCode.G))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            EndCoffeeGame();    
        }
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_CoffeeGame : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public GameObject GamePanel;

    public int frameNumber = 30;

    public float GamePanelOriginPosY;

    public float DeltaY;

    public int MachineAvailableDay;

    public GameObject ObjectScene;

    public GameObject PanelSwitcher;

    [Header("动画帧")]
    public Sprite[] LockerFrames;

    public Sprite[] RefrigeratorFrames;

    public Sprite[] GrinderFrames;

    public Sprite WrongPic;

    [Header("子项")]
    public GameObject FinishButton;

    public GameObject RestartButton;

    public GameObject Order;

    public GameObject State;

    public GameObject Cup;

    [Header("有点击动画对象")]
    public GameObject Grinder;

    public GameObject Locker;

    public GameObject Refrigerator;

    [Header("音效")]
    public AudioClip CoffeeMachineSE;

    public AudioClip GrinderMachineSE;

    //表示当前是否正在进行游戏
    [HideInInspector]
    public bool GamePlaying;


    //表示当前正在做的coffee的数据
    private S_Coffee currentCoffee;

    //表示咖啡图片是当前coffee.Sprites中的第几项
    private int currentSprite;

    //表示当前已完成的步骤数
    private int currentFinishedStepNumber;


    //表示当前步骤是否均已完成
    private bool GameFinished;

    //表示冰箱是否已经打开
    private bool RefrigeratorOpened;

    //表示柜子是否已经打开
    private bool LockerOpened;

    private void Start()
    {
        //DTList = DOTween.Sequence();
        
        //添加游戏开始事件监听
        DioLogueState ds = accessor._DioLogueState;
        ds.dialogueChanged.AddListener(StartCoffeeGame);
        
        for (int i = 0; i < ObjectScene.transform.childCount; i++)
        {
            ObjectScene.transform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
            Button b = ObjectScene.transform.GetChild(i).GetComponent<Button>();
            if (b != null)
            {
                b.interactable = false;
            }
        }

        GamePanelOriginPosY = GetComponent<RectTransform>().position.y;
        GamePlaying = false;
        RefrigeratorOpened = false;
        LockerOpened = false;
    }

    public void InitGame()
    {
        Order.GetComponent<Text>().text = currentCoffee.Name;

        RefrigeratorOpened = false;
        LockerOpened = false;

        Refrigerator.GetComponent<Image>().sprite = RefrigeratorFrames[0];
        Locker.GetComponent<Image>().sprite = LockerFrames[0];

        string stateString = "";
        for (int i = 0; i < currentCoffee.Steps.Count; i++)
        {
            stateString += accessor.DataManager.CoffeeStep.StepDescription[(int)currentCoffee.Steps[i]];
            if (i != currentCoffee.Steps.Count - 1)
            {
                stateString += "+";
            }
        }
        State.GetComponent<TextMeshProUGUI>().text = stateString;

        Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[0].Pic;
        currentSprite = 0;
        currentFinishedStepNumber = 0;
        GameFinished = false;

        for (int i = 0; i < ObjectScene.transform.childCount; i++)
        {
            Button b = ObjectScene.transform.GetChild(i).GetComponent<Button>();
            if (b != null)
            {
                b.interactable = true;
            }
        }
    }

    public void StartCoffeeGame(DiologueData data)
    {
        if (data.processState == ProcessState.Coffee && accessor._DioLogueState.state == DioState.Normal)
        {
            //控制面板移动
            RectTransform r = GetComponent<RectTransform>();

            Sequence s = DOTween.Sequence();
            s.Append(r.DOMove(new Vector2(r.position.x, r.position.y), 1.8f));
            s.Append(r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY + DeltaY), 1f));


            //YLT用来作不明觉厉功能的代码
            DioLogueState ds = accessor._DioLogueState;
            ds.SetButtonsActive(false);

            //禁用PanelSwitcher
            //for (int i = 0; i < PanelSwitcher.transform.childCount; i++)
            //{
            //    PanelSwitcher.transform.GetChild(i).GetComponent<Button>().interactable = false;
            //}

            currentCoffee = accessor.DataManager.CoffeeStep.Coffees[data.charaID - 1];
            InitGame();
            GamePlaying = true;
        }
        else
        {
            return;
        }
    }

    public void EndCoffeeGame()
    {
        if (accessor._DioLogueState.state == DioState.Normal)
        {
            //StartCoroutine(HideGamePanel());
            Sequence s = DOTween.Sequence();

            if (accessor.StateManager.State != PlaySceneState.Log)
            {
                accessor.StateManager.CancelCurrentState();
                accessor.StateManager.StateSwitchToLog();
            }

            RectTransform r = GetComponent<RectTransform>();
            s.Append(r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY), 0.8f));

            //DioLogueState ds = accessor._DioLogueState;
            //ds.SetButtonsActive(false);

            s.AppendCallback(() =>
            {
                Debug.Log("CallbackStart");
                accessor._DioLogueState.ProcessInput();
                DioLogueState ds = accessor._DioLogueState;

                if(ds.state== DioState.Normal)
                {
                    StartCoroutine(WaitToButtonActive(true));
                }
                else
                {
                    ds.SetButtonsActive(true);
                }
                //ds.SetButtonsActive(true);
                Debug.Log("CallbackEnd");
            });

            //启用PanelSwitcher
            //for (int i = 0; i < PanelSwitcher.transform.childCount; i++)
            //{
            //    PanelSwitcher.transform.GetChild(i).GetComponent<Button>().interactable = true;
            //}

            //StartCoroutine(SetButtonsActive(1.2f, true));
            GamePlaying = false;
        }

        
    }

    private IEnumerator WaitToButtonActive(bool active)
    {
        DioLogueState ds = accessor._DioLogueState;
        var c = ds.characterController;

        //等待Delay完毕才能正常点击
        yield return new WaitForSeconds(c.delay+0.2f);

        ds.SetButtonsActive(active);
    }

    /// <summary>
    /// 用于在游戏进行时终止游戏
    /// </summary>
    public void Clear()
    {
        if (GamePlaying)
        {
            GamePlaying = false;
            transform.position = new Vector3(transform.position.x, GamePanelOriginPosY, transform.position.z);
        }
        //Debug.Log("YLT是啥呗!");
    }    

    private void Update()
    {
        //测试
        if (Input.GetKeyDown(KeyCode.G))
        {
            //StartCoffeeGame();
        }
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            //EndCoffeeGame();    
        }
    }

    #region 场景物件点击事件
    //判断步骤是否已结束的通用函数，判断后执行相应操作
    public bool IsGameFinished()
    {
        if (currentFinishedStepNumber == currentCoffee.Steps.Count)
        {
            GameFinished = true;
        }
        else
        {
            GameFinished = false;
        }

        RectTransform r = GetComponent<RectTransform>();
        Sequence s = DOTween.Sequence();
        s.Append(r.DOMove(new Vector2(r.position.x, r.position.y + 5f), 0.1f));
        s.Append(r.DOMove(new Vector2(r.position.x, r.position.y - 5f), 0.1f));
        s.Append(r.DOMove(new Vector2(r.position.x, r.position.y), 0.1f));

        return GameFinished;
    }

    //点错后的通用处理函数
    public void WrongStep()
    {
        Cup.GetComponent<Image>().sprite = WrongPic;
        for (int i = 0; i < ObjectScene.transform.childCount; i++)
        {
            Button b = ObjectScene.transform.GetChild(i).GetComponent<Button>();
            if (b != null)
            {
                b.interactable = false;
            }
        }

        RectTransform r = GetComponent<RectTransform>();
        Sequence s = DOTween.Sequence();
        s.Append(r.DOMove(new Vector2(r.position.x + 5f, r.position.y), 0.1f));
        s.Append(r.DOMove(new Vector2(r.position.x - 5f, r.position.y), 0.1f));
        s.Append(r.DOMove(new Vector2(r.position.x, r.position.y), 0.1f));

        GameFinished = false;
    }

    //提交按钮点击事件
    public void OnClickFinish()
    {
        if (GameFinished)
        {
            EndCoffeeGame();
        }
    }

    //重做按钮点击事件
    public void OnClickRestart()
    {
        InitGame();
    }

    //搅拌机点击事件
    public void OnClickGrinder()
    {
        if (accessor._DioLogueState.date < MachineAvailableDay)
        {
            accessor.AudioManager.PlaySE(GrinderMachineSE);

            IEnumerator PlayGrinderAnimation()
            {
                Grinder.GetComponent<Image>().sprite = GrinderFrames[0];
                yield return new WaitForSeconds(0.5f);
                Grinder.GetComponent<Image>().sprite = GrinderFrames[1];
                yield return new WaitForSeconds(0.5f);
                Grinder.GetComponent<Image>().sprite = GrinderFrames[2];
                yield return new WaitForSeconds(0.5f);
                Grinder.GetComponent<Image>().sprite = GrinderFrames[0];
            }
            //Debug.Log("click");

            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Grind)
            {
                StartCoroutine(PlayGrinderAnimation());
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("磨豆", "<s><color=grey>磨豆</color></s>");
                if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                {
                    //Debug.Log("replace");
                    Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                    currentSprite++;
                }
                IsGameFinished();
            }
            else
            {
                WrongStep();
            }
        }
    }

    //滴滤架点击事件
    public void OnClickFunnel()
    {
        if (accessor._DioLogueState.date >= MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Extract)
            {
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("萃取", "<s><color=grey>萃取</color></s>");
                if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                {
                    //Debug.Log("replace");
                    Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                    currentSprite++;
                }
                IsGameFinished();
            }
            else
            {
                WrongStep();
            }
        }
    }

    //冷水按钮点击事件
    public void OnClickCold()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.ColdWater)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("冷水", "<s><color=grey>冷水</color></s>");
            if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
            {
                //Debug.Log("replace");
                Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                currentSprite++;
            }
            IsGameFinished();
        }
        else
        {
            WrongStep();
        }
    }

    //热水点击事件
    public void OnClickHot()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.HotWater)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("热水", "<s><color=grey>热水</color></s>");
            if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
            {
                //Debug.Log("replace");
                Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                currentSprite++;
            }
            IsGameFinished();
        }
        else
        {
            WrongStep();
        }
    }

    //冰柜点击事件
    public void OnClickRefrigerator()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Ice)
        {
            if (!RefrigeratorOpened)
            {
                Refrigerator.GetComponent<Image>().sprite = RefrigeratorFrames[1];
                RefrigeratorOpened = true;
            }
            else
            {
                Refrigerator.GetComponent<Image>().sprite = RefrigeratorFrames[0];
                RefrigeratorOpened = false;
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("冰", "<s><color=grey>冰</color></s>");
                if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                {
                    //Debug.Log("replace");
                    Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                    currentSprite++;
                }
                IsGameFinished();
            }
        }
        else
        {
            WrongStep();
        }
    }

    //柜子点击事件
    public void OnClickLocker()
    {
        if (accessor._DioLogueState.date >= MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Ice)
            {
                if (!LockerOpened)
                {
                    Locker.GetComponent<Image>().sprite = LockerFrames[1];
                    LockerOpened = true;
                }
                else
                {
                    Locker.GetComponent<Image>().sprite = LockerFrames[0];
                    LockerOpened = false;
                    currentFinishedStepNumber++;
                    State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("CAVA", "<s><color=grey>CAVA</color></s>");
                    if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                    {
                        //Debug.Log("replace");
                        Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                        currentSprite++;
                    }
                    IsGameFinished();
                }
            }
            else
            {
                WrongStep();
            }
        }
    }

    //牛奶点击事件
    public void OnClickMilk()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Milks)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("牛奶", "<s><color=grey>牛奶</color></s>");
            if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
            {
                //Debug.Log("replace");
                Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                currentSprite++;
            }
            IsGameFinished();
        }
        else
        {
            WrongStep();
        }
    }

    //手磨点击事件
    public void OnClickShouMo()
    {
        if (accessor._DioLogueState.date >= MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Grind)
            {
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("磨豆", "<s><color=grey>磨豆</color></s>");
                if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                {
                    //Debug.Log("replace");
                    Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                    currentSprite++;
                }
                IsGameFinished();
            }
            else
            {
                WrongStep();
            }
        }
    }

    //咖啡机点击事件
    public void OnClickCoffeeMachine()
    {
        if (accessor._DioLogueState.date < MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Extract)
            {
                currentFinishedStepNumber++;
                accessor.AudioManager.PlaySE(CoffeeMachineSE);
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("萃取", "<s><color=grey>萃取</color></s>");
                if (currentSprite != currentCoffee.Sprites.Count - 1 && currentCoffee.Sprites[currentSprite + 1].ChangeStep == currentFinishedStepNumber)
                {
                    //Debug.Log("replace");
                    Cup.GetComponent<Image>().sprite = currentCoffee.Sprites[currentSprite + 1].Pic;
                    currentSprite++;
                }
                IsGameFinished();
            }
            else
            {
                WrongStep();
            }
        }
    }

    #endregion
}

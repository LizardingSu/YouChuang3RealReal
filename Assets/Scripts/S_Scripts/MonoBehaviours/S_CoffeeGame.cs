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

    [Header("����֡")]
    public Sprite[] LockerFrames;

    public Sprite[] RefrigeratorFrames;

    public Sprite[] GrinderFrames;

    public Sprite WrongPic;

    [Header("����")]
    public GameObject FinishButton;

    public GameObject RestartButton;

    public GameObject Order;

    public GameObject State;

    public GameObject Cup;

    [Header("�е����������")]
    public GameObject Grinder;

    public GameObject Locker;

    public GameObject Refrigerator;

    [Header("��Ч")]
    public AudioClip CoffeeMachineSE;

    public AudioClip GrinderMachineSE;

    //��ʾ��ǰ�Ƿ����ڽ�����Ϸ
    [HideInInspector]
    public bool GamePlaying;


    //��ʾ��ǰ��������coffee������
    private S_Coffee currentCoffee;

    //��ʾ����ͼƬ�ǵ�ǰcoffee.Sprites�еĵڼ���
    private int currentSprite;

    //��ʾ��ǰ����ɵĲ�����
    private int currentFinishedStepNumber;


    //��ʾ��ǰ�����Ƿ�������
    private bool GameFinished;

    //��ʾ�����Ƿ��Ѿ���
    private bool RefrigeratorOpened;

    //��ʾ�����Ƿ��Ѿ���
    private bool LockerOpened;

    private void Start()
    {
        //DTList = DOTween.Sequence();
        
        //�����Ϸ��ʼ�¼�����
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
            //��������ƶ�
            RectTransform r = GetComponent<RectTransform>();

            Sequence s = DOTween.Sequence();
            s.Append(r.DOMove(new Vector2(r.position.x, r.position.y), 1.8f));
            s.Append(r.DOMove(new Vector2(r.position.x, GamePanelOriginPosY + DeltaY), 1f));


            //YLT�����������������ܵĴ���
            DioLogueState ds = accessor._DioLogueState;
            ds.SetButtonsActive(false);

            //����PanelSwitcher
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

            //����PanelSwitcher
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

        //�ȴ�Delay��ϲ����������
        yield return new WaitForSeconds(c.delay+0.2f);

        ds.SetButtonsActive(active);
    }

    /// <summary>
    /// ��������Ϸ����ʱ��ֹ��Ϸ
    /// </summary>
    public void Clear()
    {
        if (GamePlaying)
        {
            GamePlaying = false;
            transform.position = new Vector3(transform.position.x, GamePanelOriginPosY, transform.position.z);
        }
        //Debug.Log("YLT��ɶ��!");
    }    

    private void Update()
    {
        //����
        if (Input.GetKeyDown(KeyCode.G))
        {
            //StartCoffeeGame();
        }
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            //EndCoffeeGame();    
        }
    }

    #region �����������¼�
    //�жϲ����Ƿ��ѽ�����ͨ�ú������жϺ�ִ����Ӧ����
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

    //�����ͨ�ô�����
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

    //�ύ��ť����¼�
    public void OnClickFinish()
    {
        if (GameFinished)
        {
            EndCoffeeGame();
        }
    }

    //������ť����¼�
    public void OnClickRestart()
    {
        InitGame();
    }

    //���������¼�
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
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("ĥ��", "<s><color=grey>ĥ��</color></s>");
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

    //���˼ܵ���¼�
    public void OnClickFunnel()
    {
        if (accessor._DioLogueState.date >= MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Extract)
            {
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("��ȡ", "<s><color=grey>��ȡ</color></s>");
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

    //��ˮ��ť����¼�
    public void OnClickCold()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.ColdWater)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("��ˮ", "<s><color=grey>��ˮ</color></s>");
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

    //��ˮ����¼�
    public void OnClickHot()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.HotWater)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("��ˮ", "<s><color=grey>��ˮ</color></s>");
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

    //�������¼�
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
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("��", "<s><color=grey>��</color></s>");
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

    //���ӵ���¼�
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

    //ţ�̵���¼�
    public void OnClickMilk()
    {
        if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Milks)
        {
            currentFinishedStepNumber++;
            State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("ţ��", "<s><color=grey>ţ��</color></s>");
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

    //��ĥ����¼�
    public void OnClickShouMo()
    {
        if (accessor._DioLogueState.date >= MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Grind)
            {
                currentFinishedStepNumber++;
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("ĥ��", "<s><color=grey>ĥ��</color></s>");
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

    //���Ȼ�����¼�
    public void OnClickCoffeeMachine()
    {
        if (accessor._DioLogueState.date < MachineAvailableDay)
        {
            if (!GameFinished && currentCoffee.Steps[currentFinishedStepNumber] == S_Steps.Extract)
            {
                currentFinishedStepNumber++;
                accessor.AudioManager.PlaySE(CoffeeMachineSE);
                State.GetComponent<TextMeshProUGUI>().text = State.GetComponent<TextMeshProUGUI>().text.Replace("��ȡ", "<s><color=grey>��ȡ</color></s>");
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

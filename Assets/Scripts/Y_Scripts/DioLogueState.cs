using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//不好说
public enum DioState
{
    Normal,
    Auto,
}
public class DioLogueState : MonoBehaviour
{

    public SelectData selectdata;
    public List<string> textList = new List<string>();

    /// <summary>
    /// 记录所有已经阅读过的对话
    /// 存在一个问题，那就是对于被移除的选项Log不会进行重新加载，不能支持非线性的流程，但是解决起来很简单(已解决，性能不太好)
    /// 另一个问题，没用
    /// </summary>
    public ushort[] ReadedList;

    public DialogueWillChangeEvent dialogueWillChange;
    public DialogueChangedEvent dialogueChanged;

    //not using
    public event Func<bool> isComplete;

    public uint date;
    public string path = "Text/Day";
    public DiologueData curData;

    public Button[] update_button;

    public S_CentralAccessor centralAccessor;
    public GameObject panelSwitcher;

    //控制的东西
    public CharacterController characterController;
    public S_CoffeeGame coffee;
    public LogController logController;
    public SwitchSceneAnim switchAnim;
    public LockOutAnim lockOutAnim;

    private Coroutine c;

    public DioState state = DioState.Normal;  

    private void Awake()
    {
        foreach (var button in update_button)
            button.onClick.AddListener(ProcessInput);

        if (centralAccessor.StateManager.State != PlaySceneState.Log)
            SetButtonsActive(false);
        else
            SetButtonsActive(true);
    }
    private void OnDestroy()
    {
        textList.Clear();

        ReadedList = null;
        foreach (var button in update_button)
            button.onClick.RemoveListener(ProcessInput);
    }

    private void Update()
    {

    }

    public void LoadScene4()
    {
        Debug.Log("Scene_4");

        var pm = centralAccessor.ProcessManager;
        pm.DeleteSaving();
        pm.Load();

        date = 4;
        pm.Save(4000, 1, "");

        Init(date, path);
        switchAnim.SwitchToNewScene(3, 4);
    }

    public void LoadNewSceneImmediate()
    {
        Debug.Log("NewScene");

        var pm = centralAccessor.ProcessManager;
        pm.DeleteSaving();
        pm.Load();

//》》》》》》》要改
        date = 1;
        //date = 0;
//》》》》》》》

        Init(date, path);

//》》》》》》》要改
        switchAnim.SwitchToNewScene(0, 1);
        //xxx.xxx();
//》》》》》》》
    }
    public void ContinueGame()
    {
        Debug.Log("Continue");
        centralAccessor.MenuManager.ShowMenu(false);
        var pm = centralAccessor.ProcessManager;
        pm.Load();

        var c = pm.m_Saving1.Choices[pm.m_Saving1.Choices.Count - 1];

        ReadToCurrentID((int)(c.ID / 1000), c.ID % 1000);
    }

    public void DelayedContinueGame()
    {
        centralAccessor.TransAnim.SetActive(true);
        centralAccessor.TransAnim.GetComponent<Animator>().Play("TransAnim");
        Invoke("ContinueGame", 0.8f);
    }

    public void Init(uint day, string path)
    {
        if (textList.Count != 0) textList.Clear();

        if (date != day)
            date = day;

        var textAsset = Resources.Load<TextAsset>(path + day);
        var ta = textAsset.text.Split('\n');

        for (int i = 0; i < ta.Length; i++)
        {
            if (i != 0 && i != ta.Length - 1)
                textList.Add(ta[i]);
        }

        ReadedList = new ushort[textList.Count];
    }
    //跳转到下一条对话/选项/做咖啡
    private void UpdateDiologue()
    {
        //for test(保留下来得了，反正也懒得改了)
        var isStart = ReadedList[0] == 0;

        //保存上一句话的ID
        var lastDio = 0;

        if (curData!=null)
        {
            var isEnd = curData.nextIdx == -1;
            if (isEnd) return;
        }

        DiologueData diologueData = null;
        if (isStart)
        {
            diologueData = LogEntryParser.GetDiologueDataAtIdx(textList, 0, date);
        }
        else
        {
            diologueData = LogEntryParser.GetDiologueDataAtIdx(textList, (uint)curData.nextIdx, date);
        }



        if(curData!= null)
        {
            dialogueWillChange.Invoke(curData,diologueData);

            //保存最后一句话
            if (curData.processState == ProcessState.Diologue)
                lastDio = (int)curData.idx;
        }

        curData = diologueData;

        //当前的字段设置为已读
        ReadedList[curData.idx] = 1;

        //如果是结尾，则自动保存 转场
        if(curData.processState == ProcessState.Coffee&&curData.charaID == -1)
        {
            if(date == 0)
            {
                switchAnim.SwitchToNewScene(date, date + 1);
                return;
            }

            centralAccessor.ProcessManager.Save((int)date * 1000 + lastDio, -2, "");

            if (date%4 == 0)
            {
                var p = centralAccessor.ProcessManager;
                bool lockOut = true;

                foreach(var m in p.m_Saving1.Choices)
                {
                    if (m.Choice == 1 || m.Choice == -2 || m.ID % 1000 > date)
                        continue;

                    var r = selectdata.data.Find(x => x.SelectID == m.ID);

                    if(r == null)
                    {
                        lockOut = true;
                        break;
                    }

                    var c = r.rightChoice.Split('|');

                    bool isT = false;
                    foreach(var n in c)
                    {
                        if (int.Parse(n) == m.Choice)
                        {
                            isT = true;
                            break;
                        }
                    }

                    if (!isT)
                    {
                        lockOut = false;
                        break;
                    }

                    lockOut = isT;
                }

                if (lockOut)
                {
                    lockOutAnim.LockOutScene((int)date);
                    return;
                }
            }

            switchAnim.SwitchToNewScene(date, date + 1);
            return;
        }

        if (state == DioState.Normal&&curData.processState!=ProcessState.Branch)
            StartCoroutine(DialogueChangeEvent(curData));
        else
            dialogueChanged.Invoke(curData);


        //如果curData为Branch，自动进行到下一句话
        if (curData.processState == ProcessState.Branch)
        {
            curData.nextIdx = (int)LogEntryParser.GetNextIdxFromBranch(centralAccessor.ProcessManager.m_Saving1.Choices, curData.log);

            //嵌套，很恶心
            UpdateDiologue();
        }
    }

    //对于正常对话过程，DialogueChangeEvent要等待所有dialogueWillChange都完成
    private IEnumerator DialogueChangeEvent(DiologueData data)
    {
        //首先禁止所有点击
        SetButtonsActive(false);
        SetPanelSwitcherActive(false);

        var waitForEndCoffee = characterController.isCoffeeBefore;

        yield return new WaitUntil(isComplete);

        dialogueChanged.Invoke(data);

        //如果当前为咖啡，则不能开启点击;如果上一句是咖啡，则需要等待EndCoffeeGame才能触发;如果是第一句话，则需要等待SwitchNewScene触发
        if(waitForEndCoffee)
            SetButtonsActive(false);
        else if (curData.processState == ProcessState.Coffee)
            SetButtonsActive(false);
        else if (curData.idx == 0)
            SetButtonsActive(false);
        else 
            SetButtonsActive(true);


        //开启协程
        isReading = true;
        c = StartCoroutine(completeReading());
    }

    public bool isReading = false; 
    public void ProcessInput()
    {
        //如果是自动播放，则和ProcessInput一致
        if(state == DioState.Auto)
        {
            isReading = false;
            UpdateDiologue();
            return;
        }

        //如果当前不是正在读文本，则开启神奇妙妙协程
        if (!isReading)
        {
            UpdateDiologue();
        }
        //如果当前正在读文本，则点击时关闭生气喵喵协程并且结束所有动画
        else
        {
            logController.rightLogController.KillAllAnim();
            characterController.KillAllAnim();

            isReading = false;
            SetPanelSwitcherActive(true);

            StopCoroutine(c);
        }

    }

    private bool isCompleteReading()
    {
        if (logController.rightLogController.m_dio.maxVisibleCharacters ==
            logController.rightLogController.m_dio.textInfo.characterCount)
        {
            return true;
        }

        //if(logController.rightLogController.)

        return false;
    }

    private IEnumerator completeReading()
    {
        yield return new WaitUntil(isCompleteReading);

        if(isReading== true)
        {
            isReading = false;
            SetPanelSwitcherActive(true);
        }
    }

    private void SetPanelSwitcherActive(bool active)
    {
        foreach(var i in panelSwitcher.GetComponentsInChildren<Button>())
        {
            i.interactable= active;
        }
    }

    public void OnSelectionSelect(uint Idx, uint nextIdx,string answer = "")
    {
        curData = LogEntryParser.GetDiologueDataAtIdx(textList, Idx, date);
        curData.nextIdx = (int)nextIdx;

        //此时等于直接点击到下一句
        ProcessInput();

        if(state == DioState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(date) * 1000 + (int)Idx, (int)nextIdx,answer);
        }
    }

    //设置点击事件按钮的启动和关闭
    public void SetButtonsActive(bool active)
    {
        Debug.Log(active);
        foreach (var button in update_button)
            button.enabled = active;
    }

    //清除characterController,logController所有状态
    private void Clear()
    {
        characterController.Clear();
        logController.Clear();
        coffee.Clear();

        isReading = false;
        curData = null;
        for (int i = 0; i < ReadedList.Length; i++)
        {
            ReadedList[i] = 0;
        }
    }

    //读取到对应的ID和天数，要求对应ID是必定可访问到的
    public void ReadToCurrentID(int day, int Idx)
    {
        //首先判断需不需要重新Init
        if (textList.Count == 0 || date != day)
        {
            Init((uint)day, path);
        }

        //清除所有场景要素
        Clear();

        //如果跳转ID为-1，则直接结束
        if (Idx == -1)
        {
            return;
        }

        state = DioState.Auto;

        //第一句话
        if (curData == null)
        {
            ProcessInput();
        }

        while (curData.idx != Idx)
        {
            //如果是这两者，则说明不会自动到下一句话，所以必须触发他的下一句条件
            if (curData.processState == ProcessState.Coffee)
            {
                coffee.EndCoffeeGame();
            }
            else if (curData.processState == ProcessState.Select)
            {
                var choices = centralAccessor.ProcessManager.m_Saving1.Choices;
                foreach (var choice in choices)
                {
                    var id = choice.ID;
                    var idx = id % 1000;
                    var today = (id - idx) / 1000;
                    if (idx == curData.idx && today == curData.date)
                    {
                        OnSelectionSelect((uint)idx, (uint)choice.Choice);
                        break;
                    }
                }
            }

            ProcessInput();
        }

        //最后一句话时进行RightLogController的初始化
        logController.rightLogController.Init(logController.logEntries.Last());
        logController.RefillToButtom();

        state = DioState.Normal;
    }
}

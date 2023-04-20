using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//不好说
public enum DiologueState
{
    Normal,
    Auto,
}
public class DioLogueState : MonoBehaviour
{


    public List<string> textList = new List<string>();
    /// <summary>
    /// 记录所有已经阅读过的对话
    /// 存在一个问题，那就是对于被移除的选项Log不会进行重新加载，不能支持非线性的流程，但是解决起来很简单(已解决，性能不太好)
    /// </summary>
    public ushort[] ReadedList;
    //not Use
    public List<DiologueData> selectDatas = new List<DiologueData>();

    public DialogueWillChangeEvent dialogueWillChange;
    public DialogueChangedEvent dialogueChanged;

    public uint date;
    public DiologueData curData;

    public Button[] update_button;

    public S_CentralAccessor centralAccessor;

    public CharacterController characterController;
    public S_CoffeeGame coffee;
    public LogController logController;

    public DiologueState state = DiologueState.Normal;  

    public void Awake()
    {
        foreach (var button in update_button)
            button.onClick.AddListener(UpdateDiologue);

        if (GameObject.Find("MainManager").GetComponent<S_StateManager>().State != PlaySceneState.Log)
            SetButtonsActive(false);
        else
            SetButtonsActive(true);

        //for Test
        LoadScene(1);
    }

    //test
    private void LoadScene(uint day)
    {
        date = day;
        //test
        Init(day, "Text/T");
    }

    public void OnDestroy()
    {
        textList.Clear();

        ReadedList = null;
        foreach (var button in update_button)
            button.onClick.RemoveListener(UpdateDiologue);
    }

    public void ReadToCurrentID(int day, int Idx)
    {
        if (textList.Count == 0 || date != day)
        {
            Init((uint)day, "Text/T");
        }

        {
           Clear();
        }

        if(Idx == -1)
        {
            return;
        }

        state = DiologueState.Auto;

        if(curData == null)
        {
            UpdateDiologue();
        }

        while (curData.idx != Idx)
        {
            Debug.Log(curData.idx + "  " + Idx);

            if (curData.processState == ProcessState.Coffee)
            {
                coffee.EndCoffeeGame();
            }

            if (curData.processState == ProcessState.Select)
            {
                var choices = centralAccessor.ProcessManager.m_Saving.Choices;
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

            UpdateDiologue();
        }

        logController.rightLogController.Init(logController.logEntries.Last());
        logController.RefillToButtom();

        state = DiologueState.Normal;
    }

    public void Init(uint day,string path)
    {
        if(textList.Count!=0) textList.Clear();

        if (date != day)
            date = day;

        var textAsset = Resources.Load<TextAsset>(path+day);
        var ta = textAsset.text.Split('\n');

        for(int i = 0; i < ta.Length; i++)
        {
            if (i != 0 && i != ta.Length - 1)
                textList.Add(ta[i]);
        }

        ReadedList = new ushort[textList.Count];
    }

    //跳转到下一条对话/选项/做咖啡
    public void UpdateDiologue()
    {
        //for test
        var isStart = ReadedList[0] == 0;

        if (curData!=null)
        {
            var isEnd = curData.nextIdx == -1;
            var isSelect = curData.processState == ProcessState.Select;

            if (isEnd) return;

            //当前为选项且没有做选择
            if (isSelect && isEnd)
            {
                return;
            }

            dialogueWillChange.Invoke(curData);
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

        dialogueChanged.Invoke(diologueData);

        curData = diologueData;
        ReadedList[curData.idx] = 1;

        //如果curData为Branch，自动进行到下一句话
        if(curData.processState == ProcessState.Branch)
        {
            Debug.Log("InBranch");
            curData.nextIdx = (int)LogEntryParser.GetNextIdxFromBranch(centralAccessor.ProcessManager.m_Saving.Choices,curData.log);

            UpdateDiologue();
        }
    }

    public void OnSelectionSelect(uint Idx, uint nextIdx,string answer = "")
    {
        curData = LogEntryParser.GetDiologueDataAtIdx(textList, Idx, date);
        curData.nextIdx = (int)nextIdx;

        UpdateDiologue();

        if(state == DiologueState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(date) * 1000 + (int)Idx, (int)nextIdx,answer);
        }
    }

    public void SetButtonsActive(bool active)
    {
        foreach (var button in update_button)
            button.enabled = active;
    }

    private void Clear()
    {
        curData = null;
        characterController.Clear();
        logController.Clear();

        for(int i = 0; i < ReadedList.Length; i++)
        {
            ReadedList[i] = 0;
        }

    }
}

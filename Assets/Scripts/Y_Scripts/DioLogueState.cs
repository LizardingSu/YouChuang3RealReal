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


    public List<string> textList = new List<string>();

    /// <summary>
    /// 记录所有已经阅读过的对话
    /// 存在一个问题，那就是对于被移除的选项Log不会进行重新加载，不能支持非线性的流程，但是解决起来很简单(已解决，性能不太好)
    /// 另一个问题，没用
    /// </summary>
    public ushort[] ReadedList;

    public DialogueWillChangeEvent dialogueWillChange;
    public DialogueChangedEvent dialogueChanged;

    public uint date;
    public string path = "Text/Day";
    public DiologueData curData;

    public Button[] update_button;

    public S_CentralAccessor centralAccessor;

    //控制的东西
    public CharacterController characterController;
    public S_CoffeeGame coffee;
    public LogController logController;
    public SwitchSceneAnim switchAnim;

    public DioState state = DioState.Normal;  

    private void Awake()
    {
        foreach (var button in update_button)
            button.onClick.AddListener(UpdateDiologue);

        if (centralAccessor.StateManager.State != PlaySceneState.Log)
            SetButtonsActive(false);
        else
            SetButtonsActive(true);

        //testP
        StartCoroutine(LoadSceneWhenFirstAwake());
    }
    //test
    private IEnumerator LoadSceneWhenFirstAwake()
    {
        yield return null;

        var pm = centralAccessor.ProcessManager;
        pm.Load();
        if (pm.m_Saving.Choices.Count != 0)
        {
            date = (uint)pm.m_Saving.Choices.Last().ID / (uint)1000;
        }
        else
        {
            date = 1;
            //test
            pm.Save(1000, 1, "");
        }

        //test
        Init(date, path);
    }

    private void OnDestroy()
    {
        textList.Clear();

        ReadedList = null;
        foreach (var button in update_button)
            button.onClick.RemoveListener(UpdateDiologue);
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
    public void UpdateDiologue()
    {
        //for test
        var isStart = ReadedList[0] == 0;

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
        }

        curData = diologueData;
        ReadedList[curData.idx] = 1;

        if(curData.processState == ProcessState.Coffee&&curData.charaID == -1)
        {
            centralAccessor.ProcessManager.Save((int)(1000 * date + curData.idx-1), -1, "");

            if (date == 16)
                return;

            switchAnim.SwitchToNewScene(date, date + 1);
            return;
        }

        dialogueChanged.Invoke(curData);

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

        if(state == DioState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(date) * 1000 + (int)Idx, (int)nextIdx,answer);
        }
    }

    //设置点击事件按钮的启动和关闭
    public void SetButtonsActive(bool active)
    {
        foreach (var button in update_button)
            button.enabled = active;
    }

    //清除characterController,logController所有状态
    private void Clear()
    {
        characterController.Clear();
        logController.Clear();
        coffee.Clear();

        curData = null;
        for (int i = 0; i < ReadedList.Length; i++)
        {
            ReadedList[i] = 0;
        }
    }

    //读取到对应的ID和天数，要求对应ID是必定可访问到的
    public void ReadToCurrentID(int day, int Idx)
    {
        if (textList.Count == 0 || date != day)
        {
            Init((uint)day, path);
        }

        Clear();

        if (Idx == -1)
        {
            return;
        }

        state = DioState.Auto;

        if (curData == null)
        {
            UpdateDiologue();
        }

        while (curData.idx != Idx)
        {
            Debug.Log(curData.idx + "  " + Idx);

            //如果是这两者，则说明不会自动到下一句话，所以必须触发他的下一句条件
            if (curData.processState == ProcessState.Coffee)
            {
                coffee.EndCoffeeGame();
            }
            else if (curData.processState == ProcessState.Select)
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

        state = DioState.Normal;
    }

}

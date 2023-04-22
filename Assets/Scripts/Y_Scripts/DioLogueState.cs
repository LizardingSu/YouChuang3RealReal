using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//����˵
public enum DiologueState
{
    Normal,
    Auto,
}
public class DioLogueState : MonoBehaviour
{


    public List<string> textList = new List<string>();

    /// <summary>
    /// ��¼�����Ѿ��Ķ����ĶԻ�
    /// ����һ�����⣬�Ǿ��Ƕ��ڱ��Ƴ���ѡ��Log����������¼��أ�����֧�ַ����Ե����̣����ǽ�������ܼ�(�ѽ�������ܲ�̫��)
    /// ��һ�����⣬û��
    /// </summary>
    public ushort[] ReadedList;

    public DialogueWillChangeEvent dialogueWillChange;
    public DialogueChangedEvent dialogueChanged;

    public uint date;
    public DiologueData curData;

    public Button[] update_button;

    public S_CentralAccessor centralAccessor;

    //���ƵĶ���
    public CharacterController characterController;
    public S_CoffeeGame coffee;
    public LogController logController;

    public DiologueState state = DiologueState.Normal;  

    public void Awake()
    {
        foreach (var button in update_button)
            button.onClick.AddListener(UpdateDiologue);

        if (centralAccessor.StateManager.State != PlaySceneState.Log)
            SetButtonsActive(false);
        else
            SetButtonsActive(true);

        StartCoroutine(LoadScene());
    }
    //test
    private IEnumerator LoadScene()
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
        }

        Debug.Log(date);

        //test
        Init(date, "Text/T");
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

        Clear();

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

            //����������ߣ���˵�������Զ�����һ�仰�����Ա��봥��������һ������
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

    //��ת����һ���Ի�/ѡ��/������
    public void UpdateDiologue()
    {
        //for test
        var isStart = ReadedList[0] == 0;

        if (curData!=null)
        {
            var isEnd = curData.nextIdx == -1;
            if (isEnd) return;

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

        //���curDataΪBranch���Զ����е���һ�仰
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

    //���characterController,logController����״̬
    private void Clear()
    {
        characterController.Clear();
        logController.Clear();

        curData = null;
        for (int i = 0; i < ReadedList.Length; i++)
        {
            ReadedList[i] = 0;
        }

    }
}

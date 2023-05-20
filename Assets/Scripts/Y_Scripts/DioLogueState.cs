using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//����˵
public enum DioState
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
    public string path = "Text/Day";
    public DiologueData curData;

    public Button[] update_button;

    public S_CentralAccessor centralAccessor;

    //���ƵĶ���
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
    }
    //test
    private IEnumerator LoadNewScene()
    {
        yield return null;

        var pm = centralAccessor.ProcessManager;
        pm.DeleteSaving();
        pm.Load();

        date = 1;
        pm.Save(1000, 1, "");

        Init(date, path);
        switchAnim.SwitchToNewScene(0, 1);
    }

    public void LoadNewSceneImme()
    {
        Debug.Log("NewScene");

        var pm = centralAccessor.ProcessManager;
        pm.DeleteSaving();
        pm.Load();

        date = 1;
        pm.Save(1000, 1, "");

        Init(date, path);
        switchAnim.SwitchToNewScene(0, 1);
    }

    public void ContinueGame()
    {
        Debug.Log("Continue");
        var pm = centralAccessor.ProcessManager;
        pm.Load();

        var c = pm.m_Saving.Choices[pm.m_Saving.Choices.Count - 1];

        ReadToCurrentID((int)(c.ID / 1000), c.ID % 1000);
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
    //��ת����һ���Ի�/ѡ��/������
    public void UpdateDiologue()
    {
        //for test(�����������ˣ�����Ҳ���ø���)
        var isStart = ReadedList[0] == 0;

        //������һ�仰��ID
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

            //�������һ�仰
            if(curData.processState == ProcessState.Diologue)
                lastDio = (int)curData.idx;
        }

        curData = diologueData;
        ReadedList[curData.idx] = 1;

        //����ǽ�β�����Զ����� ת��
        if(curData.processState == ProcessState.Coffee&&curData.charaID == -1)
        {
            if (date == 16)
                return;

            centralAccessor.ProcessManager.Save((int)date*1000+lastDio, -2, "");

            switchAnim.SwitchToNewScene(date, date + 1);
            return;
        }

        dialogueChanged.Invoke(curData);

        //���curDataΪBranch���Զ����е���һ�仰
        if(curData.processState == ProcessState.Branch)
        {
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

    //���õ���¼���ť�������͹ر�
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
        coffee.Clear();

        curData = null;
        for (int i = 0; i < ReadedList.Length; i++)
        {
            ReadedList[i] = 0;
        }
    }

    //��ȡ����Ӧ��ID��������Ҫ���ӦID�Ǳض��ɷ��ʵ���
    public void ReadToCurrentID(int day, int Idx)
    {
        //�����ж��費��Ҫ����Init
        if (textList.Count == 0 || date != day)
        {
            Init((uint)day, path);
        }

        //������г���Ҫ��
        Clear();

        //�����תIDΪ-1����ֱ�ӽ���
        if (Idx == -1)
        {
            return;
        }

        state = DioState.Auto;

        //��һ�仰
        if (curData == null)
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

        //���һ�仰ʱ����RightLogController�ĳ�ʼ��
        logController.rightLogController.Init(logController.logEntries.Last());
        logController.RefillToButtom();

        state = DioState.Normal;
    }

}

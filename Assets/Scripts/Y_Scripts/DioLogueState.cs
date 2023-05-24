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

    public SelectData selectdata;
    public List<string> textList = new List<string>();

    /// <summary>
    /// ��¼�����Ѿ��Ķ����ĶԻ�
    /// ����һ�����⣬�Ǿ��Ƕ��ڱ��Ƴ���ѡ��Log����������¼��أ�����֧�ַ����Ե����̣����ǽ�������ܼ�(�ѽ�������ܲ�̫��)
    /// ��һ�����⣬û��
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

    //���ƵĶ���
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

//��������������Ҫ��
        date = 1;
        //date = 0;
//��������������

        Init(date, path);

//��������������Ҫ��
        switchAnim.SwitchToNewScene(0, 1);
        //xxx.xxx();
//��������������
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
    //��ת����һ���Ի�/ѡ��/������
    private void UpdateDiologue()
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
            if (curData.processState == ProcessState.Diologue)
                lastDio = (int)curData.idx;
        }

        curData = diologueData;

        //��ǰ���ֶ�����Ϊ�Ѷ�
        ReadedList[curData.idx] = 1;

        //����ǽ�β�����Զ����� ת��
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


        //���curDataΪBranch���Զ����е���һ�仰
        if (curData.processState == ProcessState.Branch)
        {
            curData.nextIdx = (int)LogEntryParser.GetNextIdxFromBranch(centralAccessor.ProcessManager.m_Saving1.Choices, curData.log);

            //Ƕ�ף��ܶ���
            UpdateDiologue();
        }
    }

    //���������Ի����̣�DialogueChangeEventҪ�ȴ�����dialogueWillChange�����
    private IEnumerator DialogueChangeEvent(DiologueData data)
    {
        //���Ƚ�ֹ���е��
        SetButtonsActive(false);
        SetPanelSwitcherActive(false);

        var waitForEndCoffee = characterController.isCoffeeBefore;

        yield return new WaitUntil(isComplete);

        dialogueChanged.Invoke(data);

        //�����ǰΪ���ȣ����ܿ������;�����һ���ǿ��ȣ�����Ҫ�ȴ�EndCoffeeGame���ܴ���;����ǵ�һ�仰������Ҫ�ȴ�SwitchNewScene����
        if(waitForEndCoffee)
            SetButtonsActive(false);
        else if (curData.processState == ProcessState.Coffee)
            SetButtonsActive(false);
        else if (curData.idx == 0)
            SetButtonsActive(false);
        else 
            SetButtonsActive(true);


        //����Э��
        isReading = true;
        c = StartCoroutine(completeReading());
    }

    public bool isReading = false; 
    public void ProcessInput()
    {
        //������Զ����ţ����ProcessInputһ��
        if(state == DioState.Auto)
        {
            isReading = false;
            UpdateDiologue();
            return;
        }

        //�����ǰ�������ڶ��ı���������������Э��
        if (!isReading)
        {
            UpdateDiologue();
        }
        //�����ǰ���ڶ��ı�������ʱ�ر���������Э�̲��ҽ������ж���
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

        //��ʱ����ֱ�ӵ������һ��
        ProcessInput();

        if(state == DioState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(date) * 1000 + (int)Idx, (int)nextIdx,answer);
        }
    }

    //���õ���¼���ť�������͹ر�
    public void SetButtonsActive(bool active)
    {
        Debug.Log(active);
        foreach (var button in update_button)
            button.enabled = active;
    }

    //���characterController,logController����״̬
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
            ProcessInput();
        }

        while (curData.idx != Idx)
        {
            //����������ߣ���˵�������Զ�����һ�仰�����Ա��봥��������һ������
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

        //���һ�仰ʱ����RightLogController�ĳ�ʼ��
        logController.rightLogController.Init(logController.logEntries.Last());
        logController.RefillToButtom();

        state = DioState.Normal;
    }
}

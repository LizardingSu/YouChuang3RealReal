using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//跟文本对话相关的部分（不包括做咖啡和场景所需要的数据）
public class LogEntry
{
    //天数
    public uint Day;
    public uint Idx;
    //Log状态，包括是左侧还是右侧，是否是选项
    public bool Left;
    public bool Select;
    //文本信息：名字，内容
    public string Name;
    public string Log;

    public LogEntry(uint day,uint idx, bool left, bool select, string name, string log)
    {
        Day = day;
        Idx = idx;
        Left = left;
        Select = select;
        Name = name;
        Log = log;
    }
}
public class LogController : MonoBehaviour,LoopScrollPrefabSource, LoopScrollDataSource
{
    public S_CentralAccessor centralAccessor;
    private DioLogueState diologueState;

    public LogEntryController logEntryPrefab;
    private LoopScrollRect scrollRect;

    public List<LogEntry> logEntries = new List<LogEntry>();

    //button for test
    private Button panel_button;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<LoopScrollRect>();
        scrollRect.prefabSource = this;
        scrollRect.dataSource = this;

        diologueState = centralAccessor._DioLogueState;
        diologueState.dialogueChanged.AddListener(onDiologueChanged);
        diologueState.dialogueWillChange.AddListener(onDiologueWillChange);
    }

    private void OnDestroy()
    {
        diologueState.dialogueChanged.RemoveListener(onDiologueChanged);
        diologueState.dialogueWillChange.RemoveListener(onDiologueWillChange);
    }

    private void onDiologueWillChange(DiologueData diologueData)
    {
        if (diologueData.isSelect)
        {
            RemoveRange(diologueData.idx);
        }
    }

    private void onDiologueChanged(DiologueData diologueData)
    {
        if (diologueData.processState == ProcessState.Coffee) return;

        bool left = diologueData.charaID != (diologueData.charaID & 1);

        AddEntry(diologueData.date, diologueData.idx, left, diologueData.isSelect, diologueData.name, diologueData.log);
    }

    public void RemoveRange(uint Idx)
    {
        logEntries.RemoveAll(x => x.Idx > Idx);

        scrollRect.totalCount = logEntries.Count;

    }

    public void AddEntry(uint date,uint idx,bool left,bool select,string name,string log)
    {

        logEntries.Add(new LogEntry(date,idx,left,select,name,log));

        //我根本不知道这个是什么原理，但他就是能正常运作，位置和顺序都不能变，我觉得跟帧数有关，等再次出bug我再改...
        RefillToButtom();
        //scrollRect.SrollToCell(logEntries.Count - 1, 10000);
        scrollRect.verticalNormalizedPosition = 1;

    }
    private void RefillToButtom()
    {
        scrollRect.totalCount = logEntries.Count;
        scrollRect.RefillCellsFromEnd();
        scrollRect.verticalNormalizedPosition = 1;
    }

    #region LoopScrollRect
    Stack<Transform> pool = new Stack<Transform>();

    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            var element =  Instantiate(logEntryPrefab,scrollRect.content);
            return element.gameObject;
        }
        Transform candidate = pool.Pop();
        candidate.SetParent(scrollRect.content, false);
        var go = candidate.gameObject;
        go.SetActive(true);
        return go;
    }

    public void ReturnObject(Transform trans)
    {
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    /// <summary>
    /// 注意，这里的isFirstTime的运行逻辑是scrollRect.RefillCellsFromEnd()会遍历并调用ProvideData函数
    /// 因此在这次provideData时ReadedList还没有更新，所以此时可以视作是第一次出现该对话
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="idx"></param>
    public void ProvideData(Transform transform, int idx)
    {
        var logEntry = logEntries[idx];

        var isFirstTime = diologueState.ReadedList[logEntry.Idx] == 0;

        var logEntryController = transform.GetComponent<LogEntryController>();
        logEntryController.Init(logEntry, isFirstTime);
    }
    #endregion

}

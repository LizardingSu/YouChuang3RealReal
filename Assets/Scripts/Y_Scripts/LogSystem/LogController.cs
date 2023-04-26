using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public RightLogController rightLogController;
    /// <summary>
    /// 多种情况时间：
    /// 升起：delay+showTime
    /// 收起：hideTime
    /// </summary>
    public float hideTime = 0.5f;
    public float showTime = 0.5f;
    [Tooltip("控制的是重新升起对话框之前的等待时间")]
    public float delay = 0.6f;

    public LogEntryController logEntryPrefab;
    private LoopScrollRect scrollRect;

    public List<LogEntry> logEntries = new List<LogEntry>();

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

    private void onDiologueWillChange(DiologueData diologueData,DiologueData diologueData1)
    {
        //前提，做咖啡做完之后必进对话，不然要改 如果前一个是做咖啡，则需要把Panel移动下去
        if(diologueData.processState == ProcessState.Coffee)
        {
            //延迟升起，但是在auto模式下不需要收起，因为没有必要
            if (diologueState.state == DioState.Normal)
                StartCoroutine(MoveUpRightPanel(delay));
        }
    }

    private void onDiologueChanged(DiologueData diologueData)
    {
        //如果是快进模式 就不需要保存
        if (diologueData.processState == ProcessState.Select&&diologueState.state == DioState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(diologueData.date) * 1000 + (int)diologueData.idx, -1,"");
        }
        else if (diologueData.processState == ProcessState.Coffee)
        {
            //做咖啡的时候收起，但是在auto模式下不需要收起，因为没有必要
            if(diologueState.state == DioState.Normal)
            {
                rightLogController.MoveDown(hideTime);
            }

            return;
        }
        else if(diologueData.processState == ProcessState.Branch)
        {
            return;
        }

        if (diologueData.log == "") return;

        bool left = diologueData.charaID != (diologueData.charaID & 1);
        bool isSelect = diologueData.processState == ProcessState.Select;

        logEntries.Add(new LogEntry(diologueData.date, diologueData.idx, left, isSelect, diologueData.name, diologueData.log));
        //当读取的时候，只需要最后一句话的时候init一下就可以
        if(diologueState.state == DioState.Normal)
        {
            RefillToButtom();
            rightLogController.Init(logEntries.Last());
        }

        //第一次阅读
        if (diologueData.idx == 0&&diologueState.state == DioState.Normal)
        {       
            //延迟升起
            StartCoroutine(MoveUpRightPanel(delay));
        }
        else
        {
            StartCoroutine(MoveUpRightPanel(0));
        }
    }
    private IEnumerator MoveUpRightPanel(float time)
    {
        yield return new WaitForSeconds(time);
        rightLogController.MoveUp(showTime);
    }

    public void RefillToButtom()
    {
        StartCoroutine(Refill());
    }
    private IEnumerator Refill()
    {
        yield return null;
        yield return null;
        yield return null;
        scrollRect.totalCount = logEntries.Count;
        scrollRect.RefillCellsFromEnd();
        scrollRect.verticalNormalizedPosition = 1;
    }

    //清除LogEntries，ScrollRect里的所有Cells
    public void Clear()
    {
        //rightPanel下移
        rightLogController.MoveDown(0);

        //聊天记录全部刷新
        logEntries.Clear();
        scrollRect.ClearCells();
        RefillToButtom();
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

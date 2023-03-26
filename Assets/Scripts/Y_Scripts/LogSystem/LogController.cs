using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LogEntry
{
    //天数
    public uint Day;
    //Log状态，包括是左侧还是右侧，是否是选项
    public bool Left;
    public bool Select;
    //文本信息：名字，内容
    public string Name;
    public string Log;

    public LogEntry(uint day, bool left, bool select, string name, string log)
    {
        Day = day;
        Left = left;
        Select = select;
        Name = name;
        Log = log;
    }
}
public class LogController : MonoBehaviour,LoopScrollPrefabSource, LoopScrollDataSource
{
    private DioLogueState diologueState;

    public LogEntryController logEntryPrefab;
    private LoopScrollRect scrollRect;

    private Button panel_Button;
    public List<LogEntry> logEntries = new List<LogEntry>();

    private void Awake()
    {
        scrollRect = GetComponentInChildren<LoopScrollRect>();
        scrollRect.prefabSource = this;
        scrollRect.dataSource = this;

        //for test
        panel_Button = GetComponent<Button>();
        panel_Button.onClick.AddListener(AddEntry);
    }

    private void OnDestroy()
    {
        panel_Button.onClick.RemoveListener(AddEntry);
    }

    static int left = 1;
    private void AddEntry()
    {
        bool _left = left % 2 == 0;
        logEntries.Add(new LogEntry(1,_left,false,"TestName"+left, "TestDioTestDioTestDioTestDioTestDioTestDioTestDio-"));
        left++;

        scrollRect.totalCount = logEntries.Count;
        scrollRect.RefillCellsFromEnd();
    }

    private void Update()
    {
        Debug.Log(logEntries.Count);
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

    public void ProvideData(Transform transform, int idx)
    {
        //transform.SendMessage("ScrollCellIndex", idx);
        var logEntry = logEntries[idx];

        var logEntryController = transform.GetComponent<LogEntryController>();
        logEntryController.Init(idx,logEntry,true);
    }
    #endregion

}

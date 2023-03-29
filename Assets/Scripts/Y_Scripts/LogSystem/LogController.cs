using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//���ı��Ի���صĲ��֣������������Ⱥͳ�������Ҫ�����ݣ�
public class LogEntry
{
    //����
    public uint Day;
    public uint Idx;
    //Log״̬����������໹���Ҳ࣬�Ƿ���ѡ��
    public bool Left;
    public bool Select;
    //�ı���Ϣ�����֣�����
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

        //�Ҹ�����֪�������ʲôԭ����������������������λ�ú�˳�򶼲��ܱ䣬�Ҿ��ø�֡���йأ����ٴγ�bug���ٸ�...
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
    /// ע�⣬�����isFirstTime�������߼���scrollRect.RefillCellsFromEnd()�����������ProvideData����
    /// ��������provideDataʱReadedList��û�и��£����Դ�ʱ���������ǵ�һ�γ��ָöԻ�
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

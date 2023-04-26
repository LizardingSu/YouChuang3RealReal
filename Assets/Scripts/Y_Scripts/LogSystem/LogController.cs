using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public RightLogController rightLogController;
    /// <summary>
    /// �������ʱ�䣺
    /// ����delay+showTime
    /// ����hideTime
    /// </summary>
    public float hideTime = 0.5f;
    public float showTime = 0.5f;
    [Tooltip("���Ƶ�����������Ի���֮ǰ�ĵȴ�ʱ��")]
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
        //ǰ�ᣬ����������֮��ؽ��Ի�����ȻҪ�� ���ǰһ���������ȣ�����Ҫ��Panel�ƶ���ȥ
        if(diologueData.processState == ProcessState.Coffee)
        {
            //�ӳ����𣬵�����autoģʽ�²���Ҫ������Ϊû�б�Ҫ
            if (diologueState.state == DioState.Normal)
                StartCoroutine(MoveUpRightPanel(delay));
        }
    }

    private void onDiologueChanged(DiologueData diologueData)
    {
        //����ǿ��ģʽ �Ͳ���Ҫ����
        if (diologueData.processState == ProcessState.Select&&diologueState.state == DioState.Normal)
        {
            centralAccessor.ProcessManager.Save((int)(diologueData.date) * 1000 + (int)diologueData.idx, -1,"");
        }
        else if (diologueData.processState == ProcessState.Coffee)
        {
            //�����ȵ�ʱ�����𣬵�����autoģʽ�²���Ҫ������Ϊû�б�Ҫ
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
        //����ȡ��ʱ��ֻ��Ҫ���һ�仰��ʱ��initһ�¾Ϳ���
        if(diologueState.state == DioState.Normal)
        {
            RefillToButtom();
            rightLogController.Init(logEntries.Last());
        }

        //��һ���Ķ�
        if (diologueData.idx == 0&&diologueState.state == DioState.Normal)
        {       
            //�ӳ�����
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

    //���LogEntries��ScrollRect�������Cells
    public void Clear()
    {
        //rightPanel����
        rightLogController.MoveDown(0);

        //�����¼ȫ��ˢ��
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Windows.WebCam.VideoCapture;

public class DioLogueState : MonoBehaviour
{
    //不好说
    public enum State
    {
        Normal,
        Auto,
        FastForward
    }

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

    public Button panel_button;

    public void Awake()
    {
        //test
        Init(0, "Text/test");

        panel_button.onClick.AddListener(UpdateDiologue);
    }

    public void OnDestroy()
    {
        textList.Clear();

        ReadedList = null;
        panel_button.onClick.RemoveListener(UpdateDiologue);
    }

    public void Init(uint day,string path)
    {
        if(textList.Count!=0) textList.Clear();

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
    private void UpdateDiologue()
    {
        //for test
        var isStart = ReadedList[0] == 0;

        if (curData!=null)
        {
            var isEnd = curData.nextIdx == -1;
            var isSelect = curData.isSelect == true;

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
    }

    public void OnSelectionSelect(uint Idx, uint nextIdx)
    {
        curData = LogEntryParser.GetDiologueDataAtIdx(textList, Idx, date);
        curData.nextIdx = (int)nextIdx;

        UpdateDiologue();
    }
}

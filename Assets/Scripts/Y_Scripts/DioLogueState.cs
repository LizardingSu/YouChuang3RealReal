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
    //����˵
    public enum State
    {
        Normal,
        Auto,
        FastForward
    }

    public List<string> textList = new List<string>();
    /// <summary>
    /// ��¼�����Ѿ��Ķ����ĶԻ�
    /// ����һ�����⣬�Ǿ��Ƕ��ڱ��Ƴ���ѡ��Log����������¼��أ�����֧�ַ����Ե����̣����ǽ�������ܼ�(�ѽ�������ܲ�̫��)
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

    //��ת����һ���Ի�/ѡ��/������
    private void UpdateDiologue()
    {
        //for test
        var isStart = ReadedList[0] == 0;

        if (curData!=null)
        {
            var isEnd = curData.nextIdx == -1;
            var isSelect = curData.isSelect == true;

            if (isEnd) return;

            //��ǰΪѡ����û����ѡ��
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

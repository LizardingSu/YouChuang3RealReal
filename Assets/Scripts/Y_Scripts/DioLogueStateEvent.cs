using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterState
{
    In,
    Permanent,
    Leave
}

public enum ProcessState
{
    Coffee,
    Select,
    Diologue
}
//��Ϸ��������Ҫ����������
public class DiologueData
{
    //��ǰ����
    public uint date;
    //DiologueData��������״̬
    public ProcessState processState;

    //���ĵ��е�λ���Լ���һ����ȡ�����ѡ�����nextIdx�����壩
    public uint idx;
    public int nextIdx;


    //���볡���
    public int charaID;
    public int emojiID;
    public CharacterState charaState;


    //ѡ�����
    public bool isSelect;

    //�ı���Ϣ
    public string name;
    public string log;

    //todo:animation

    public DiologueData(uint date, ProcessState processState, uint idx, int nextIdx, int charaID, int emojiID, CharacterState charaState, bool isSelect, string name, string log)
    {
        this.date = date;
        this.processState = processState;
        this.idx = idx;
        this.nextIdx = nextIdx;
        this.charaID = charaID;
        this.emojiID = emojiID;
        this.charaState = charaState;
        this.isSelect = isSelect;
        this.name = name;
        this.log = log;
    }
}

[Serializable]
public class DialogueChangedEvent : UnityEvent<DiologueData> { }

[Serializable]
public class DialogueWillChangeEvent : UnityEvent<DiologueData> { }
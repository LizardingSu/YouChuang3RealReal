using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterState
{
    None,
    In,
    Leave
}

public enum ProcessState
{
    Coffee,
    Select,
    Diologue,
    Branch
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

    //�ı���Ϣ
    public string name;
    public string log;

    public string resource;
    //todo:animation

    public DiologueData(uint date, ProcessState processState, uint idx, int nextIdx, int charaID, int emojiID, CharacterState charaState, string name, string log,string resource)
    {
        this.date = date;
        this.processState = processState;
        this.idx = idx;
        this.nextIdx = nextIdx;
        this.charaID = charaID;
        this.emojiID = emojiID;
        this.charaState = charaState;
        this.name = name;
        this.log = log;
        this.resource = resource;
    }
}

[Serializable]
public class DialogueChangedEvent : UnityEvent<DiologueData> { }

[Serializable]
public class DialogueWillChangeEvent : UnityEvent<DiologueData,DiologueData> { }
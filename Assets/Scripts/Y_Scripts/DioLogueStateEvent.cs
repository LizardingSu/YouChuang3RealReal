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
//游戏进程所需要的所有数据
public class DiologueData
{
    //当前天数
    public uint date;
    //DiologueData所包括的状态
    public ProcessState processState;

    //在文档中的位置以及下一个读取项（对于选项而言nextIdx无意义）
    public uint idx;
    public int nextIdx;


    //出入场相关
    public int charaID;
    public int emojiID;
    public CharacterState charaState;

    //文本信息
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
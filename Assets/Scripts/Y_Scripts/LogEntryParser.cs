using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class LogEntryParser
{
    /// <summary>
    /// *对于选项：DiologueData中是第一个的Idx，nextIdx为-1，表明当前不能跳转
    /// ta[0]:标示 #:对话 $:咖啡 &:选项 ？（非空）
    /// ta[1]:对话ID 一次只会出现一个 表明了在Excel里的位置（非空）
    /// ta[2]:人物ID 可以为空 会标注为-1 除了做咖啡和结尾应该不会出现这种情况？
    /// ta[3]:表情ID 可以为空 为空为-1 除了做咖啡和结尾不会出现这种情况？ 控制人物立绘
    /// ta[4]:名字 只有放到Text里的作用 可以为空？
    /// ta[5]:内容 对话内容 含富文本 对于Input类型的会有@ @的情况 可以为空？ *对于选项会将多个内容合并成一个Log然后解析 *对于分支会做一个妙妙的合并
    /// ta[6]:跳转 最后一句话，选择中有Input的选项，分支情况为-1 *对于选项而言在生成DiologueData时会设为-1，等待后续选择选项之后再设为正常
    /// ta[7]:出入场 I:进入 D;离去 P：固定位置
    /// ta[8]:效果
    /// *咖啡+charID为空代表是最后一句话
    /// </summary>
    /// <param name="textLists"></param>
    /// <param name="curIdx"></param>
    /// <param name="date"></param>
    /// <param name="isFirstTime"></param>
    /// <returns></returns>
    public static DiologueData GetDiologueDataAtIdx(List<string> textLists,uint curIdx,uint date)
    {
        var curtext = textLists[(int)curIdx];
        var ta = curtext.Split(',');

        var processState = ProcessState.Diologue;


        var state = ta[0];
        var charaPos = ta[7];

        var idx = uint.Parse(ta[1]);
        var nextIdx = ta[6]==""?-1:int.Parse(ta[6]);
        var charId = ta[2]==""?-1:int.Parse(ta[2]);
        //to do（控制人物立绘）
        var emojiId = ta[3] == "" ? -1 : int.Parse(ta[3]);

        var name = ta[4];
        var log = ta[5];
        //select
        if (state == "&")
        {
            processState = ProcessState.Select;

            //将所有选项相关的话合并在一起然后等解析
            var totalLog = "";
            for(int i = (int)curIdx; textLists[i][0] == '&'; i++)
            {
                var data = textLists[i].Split(',');
                //example: |this is A option^10|this is B option^11|Inpu@C-12@ord^0
                var st = data[6] != "" ? data[6] : Convert.ToString(0);
                totalLog += "|"+data[5]+"^"+ st;
            }

            log = totalLog;

            nextIdx = -1;

        }
        if(state == "*")
        {
            processState=ProcessState.Branch;
        }
        if (state == "$")
        {
            processState = ProcessState.Coffee;
        }

        var characterState = CharacterState.None;
        switch (charaPos)
        {
            case "None":
                characterState = CharacterState.None;
                break;
            case "In":
                characterState = CharacterState.In;
                break;
            case "Out":
                characterState = CharacterState.Leave;
                break;
        }

        return new DiologueData(date, processState,idx, nextIdx, charId, emojiId, characterState, name, log);
    }

    public static IReadOnlyList<SelectContent> GetSelectContents(string Log)
    {
        List<SelectContent> contents = new List<SelectContent>();

        var ta = Log.Split('|');
        //ta[0]为空
        for(int i = 1; i < ta.Length; i++)
        {
            var data = ta[i].Split('^');
            //有@则表示是Input
            contents.Add(new SelectContent(data[0].Contains('@'), data[0], uint.Parse(data[1])));
        }

        return contents;
    }

    public static uint GetNextIdxFromBranch(List<S_ChoiceMade> Choices,string Log)
    {
        var dif = Log.Split('|');
        foreach(var s in dif)
        {
            var m = s.Split('=');
            foreach (var choice in Choices)
            {
                if(choice.ID == int.Parse(m[0]))
                {
                    var n = m[1].Split('^');
                    if (int.Parse(n[0]) == choice.Choice)
                    {
                        Debug.Log(n[1]);
                        return uint.Parse(n[1]);
                    }
                }
            }
        }

        return 0;
    }
}

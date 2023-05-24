using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    BGM,
    SE,
    CG,
    VIDEO
}

public enum ResourcePlace
{
    Before,
    In,
    After
}

public struct GameResource
{
    public ResourcePlace resourcePlace;
    public ResourceType resourceType; 
    public string path; 

    public GameResource(ResourcePlace place,ResourceType type,string p)
    {
        resourcePlace= place;
        resourceType= type;
        path= p;
    }
}

[System.Serializable]
public class RightChoice
{
    public int SelectID;
    public string rightChoice;

    public RightChoice(int s,string r)
    {
        SelectID = s;
        rightChoice = r;
    }
}

static public class LogEntryParser
{
    /// <summary>
    /// *����ѡ�DiologueData���ǵ�һ����Idx��nextIdxΪ-1��������ǰ������ת
    /// ta[0]:��ʾ #:�Ի� $:���� &:ѡ�� �����ǿգ�
    /// ta[1]:�Ի�ID һ��ֻ�����һ�� ��������Excel���λ�ã��ǿգ�
    /// ta[2]:����ID ����Ϊ�� ���עΪ-1 ���������Ⱥͽ�βӦ�ò���������������
    /// ta[3]:����ID ����Ϊ�� Ϊ��Ϊ-1 ���������Ⱥͽ�β���������������� ������������
    /// ta[4]:���� ֻ�зŵ�Text������� ����Ϊ�գ�
    /// ta[5]:���� �Ի����� �����ı� ����Input���͵Ļ���@ @����� ����Ϊ�գ� *����ѡ��Ὣ������ݺϲ���һ��LogȻ����� *���ڷ�֧����һ������ĺϲ�
    /// ta[6]:��ת ���һ�仰��ѡ������Input��ѡ���֧���Ϊ-1 *����ѡ�����������DiologueDataʱ����Ϊ-1���ȴ�����ѡ��ѡ��֮������Ϊ����
    /// ta[7]:���볡 I:���� D;��ȥ P���̶�λ��
    /// ta[8]:Ч��
    /// *����+charIDΪ�մ��������һ�仰
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
        //to do�������������棩
        var emojiId = ta[3] == "" ? -1 : int.Parse(ta[3]);

        var name = ta[4];
        var log = ta[5];
        var resource = ta[8];
        //select
        if (state == "&")
        {
            processState = ProcessState.Select;

            //������ѡ����صĻ��ϲ���һ��Ȼ��Ƚ���
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

        return new DiologueData(date, processState,idx, nextIdx, charId, emojiId, characterState, name, log, resource);
    }

    public static IReadOnlyList<SelectContent> GetSelectContents(string Log)
    {
        List<SelectContent> contents = new List<SelectContent>();

        var ta = Log.Split('|');
        //ta[0]Ϊ��
        for(int i = 1; i < ta.Length; i++)
        {
            var data = ta[i].Split('^');
            //��@���ʾ��Input
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

    public static IReadOnlyList<GameResource> GetResourceTypeAndResource(string resource)
    {
        List<GameResource> list = new List<GameResource>(); 

        if(resource.Length < 2)
            return list;

        var l = resource.Split('-');

        foreach(var m in l)
        {
            var type = ResourceType.CG;
            var place = ResourcePlace.Before;

            var p = m.Split('|');

            var path = p[0];
            var k = p[1];



            switch (k[0])
            {
                case 'c':
                    type = ResourceType.CG;
                    break;
                case 'b':
                    type = ResourceType.BGM;
                    break;
                case 'm':
                    type= ResourceType.SE;
                    break;
                case 'v':
                    type = ResourceType.VIDEO;
                    break;
            }

            switch (k[1])
            {
                case 'b':
                    place = ResourcePlace.Before;
                    break;
                case 'i':
                    place = ResourcePlace.In;
                    break;
                case 'a':
                    place = ResourcePlace.After; 
                    break;
            }

            list.Add(new GameResource(place, type, path));
        }

        return list;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class LogEntryParser
{
    /// <summary>
    /// *����ѡ�DiologueData���ǵ�һ����Idx��nextIdxΪ-1��������ǰ������ת
    /// ta[0]:��ʾ #:�Ի� $:���� &:ѡ�� �����ǿգ�
    /// ta[1]:�Ի�ID һ��ֻ�����һ�� ��������Excel���λ�ã��ǿգ�
    /// ta[2]:����ID ����Ϊ�� ���עΪ-1 ���������Ⱥͽ�βӦ�ò���������������
    /// ta[3]:����ID ����Ϊ�� Ϊ��Ϊ-1 ���������Ⱥͽ�β���������������� ������������
    /// ta[4]:���� ֻ�зŵ�Text������� ����Ϊ�գ�
    /// ta[5]:���� �Ի����� �����ı� ����Input���͵Ļ���@ @����� ����Ϊ�գ� *����ѡ��Ὣ������ݺϲ���һ��LogȻ�����
    /// ta[6]:��ת �������һ�仰���⣬������Ϊ�� *����ѡ�����������DiologueDataʱ����Ϊ-1���ȴ�����ѡ��ѡ��֮������Ϊ����
    /// ta[7]:���볡 I:���� D;��ȥ P���̶�λ��
    /// ta[8]:Ч��
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

        bool isSelect = false;
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
        //select
        if (state == "&")
        {
            processState = ProcessState.Select;
            isSelect = true;
            //������ѡ����صĻ��ϲ���һ��Ȼ��Ƚ���
            var totalLog = "";
            for(int i = (int)curIdx; textLists[i][0] == '&'; i++)
            {
                var data = textLists[i].Split(',');
                //example: |this is A option^10|this is B option^11|Inpu@tW@ord^12
                totalLog += "|"+data[5]+"^"+ data[6];
            }

            log = totalLog;

            nextIdx = -1;

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

        return new DiologueData(date, processState,idx, nextIdx, charId, 0, characterState, name, log);
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
}

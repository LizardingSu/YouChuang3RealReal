using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterController : MonoBehaviour
{
    public S_CentralAccessor central;
    private DioLogueState diologueState;

    public CharacterEntryController left;
    public CharacterEntryController right;
    public WhiteAnim whiteMask;

    /// <summary>
    /// �����������ʱ��
    /// ����ʾ���ֵ�����ʾ���֣�hideNameTime
    /// �Ӳ���ʾ���ֵ���ʾ���֣�showNameTime
    /// ����ײ�����ʾ���֣�showAllTime
    /// ����ʾ���ֵ���ײ���hideAllTime
    /// </summary>
    public float hideNameTime = 0.2f;
    public float showNameTime = 0.2f;
    public float showAllTime = 1.0f;
    public float hideAllTime = 1.0f;
    public float fadeTime = 0.15f;

    [Tooltip("���Ƶ�����������Character��֮ǰ�ĵȴ�ʱ��")]
    public float delay = 0.6f;

    [Tooltip("���Ƶ��ǽ��а��ֵ�һ������ʧ֮ǰ�ĵȴ�ʱ��")]
    public float whiteFoldDelay = 1.0f;

    private bool isCoffeeBefore = false;

    public List<CharacterEntryController> windowsCharacters = new List<CharacterEntryController>(3);
    public List<int> charSortedList = new List<int>(3);

    public ConfigCharacterFile characterFiles;

    //���Ƶ�ǰ����Ĵ���
    private int curPlace = 0;


    private void Awake()
    {
        left.CharID = -1;
        left.ImageID = -1;
        right.CharID = -1;
        right.ImageID = -1;

        for (int i = 0; i < windowsCharacters.Count; i++)
        {
            windowsCharacters[i].CharID = -1;
            windowsCharacters[i].ImageID = -1;
            windowsCharacters[i].gameObject.SetActive(false);
        }

        diologueState = central._DioLogueState;
        diologueState.dialogueWillChange.AddListener(WillChangeCharacter);
        diologueState.dialogueChanged.AddListener(ChangeCharacters);
    }

    private void OnDestroy()
    {
        diologueState.dialogueWillChange.RemoveListener(WillChangeCharacter);
        diologueState.dialogueChanged.RemoveListener(ChangeCharacters);
    }

    public void Clear()
    {
        curPlace = 0;
        charSortedList.Clear();

        foreach(var c in windowsCharacters)
            c.SetAllDatas(false);

        //ȫ���ƶ���ȥ
        left.MoveDown(0);
        left.SetAllDatas(true);
        right.MoveDown(0);
        right.SetAllDatas(true);

        whiteMask.mask = 0;
    }
   
    //�޷�����ػ����,����û���������
   private void WillChangeCharacter(DiologueData data,DiologueData data2)
    {
        //����������ǶԻ��ͻ����
        if (data.processState == ProcessState.Coffee)
        {
            //�������������ʱ������Ҫ��Ļ����ʧ�������������Ҫ
            if(diologueState.state == DioState.Normal)
            {
                whiteMask.FadeAndReFill(false);
            }
            //����֮ǰ�ǲ��ǿ������жϺ���
            isCoffeeBefore = true;
        }
        else if(data.processState == ProcessState.Diologue||data.processState == ProcessState.Select)
        {
            if(data2.processState == ProcessState.Diologue&&data2.charaState == CharacterState.In)
            {

                if (diologueState.state != DioState.Normal) return;

                if (data2.idx > 1 && data.idx > 1)
                {
                    left.FadeAnim(fadeTime);
                }
                else if (data2.idx <= 1 && data.idx <= 1)
                {
                    right.FadeAnim(fadeTime);
                }
            }
        }
    }

   private void ChangeCharacters(DiologueData data)
    {
        //����ǳ�ʼ���׶�
        if(data.idx == 0)
        {
            whiteMask.FadeAndReFill(false);
        }

        //�����ǰ��coffee���
        if (diologueState.state == DioState.Normal)
        {
            //����������Ƚ׶Σ�����
            if (data.processState == ProcessState.Coffee)
            {
                if (left.curState == CurState.HIDE)
                    left.MoveDown(hideAllTime - hideNameTime);
                else
                    left.MoveDown(hideAllTime);

                if (right.curState == CurState.HIDE)
                    right.MoveDown(hideAllTime - hideNameTime);
                else
                    right.MoveDown(hideAllTime);

                StartCoroutine(FoldWhite(whiteFoldDelay));
                return;
            }

            if (isCoffeeBefore)
            {
                isCoffeeBefore = false;
                StartCoroutine(ShowUpAfterCoffee(data, delay));
                return;
            }
        }
        else
        {
            if (isCoffeeBefore) isCoffeeBefore = false;
        }

        //����ǶԻ�����ѡ��׶βŽ���
        if(data.processState == ProcessState.Select||data.processState == ProcessState.Diologue) 
        {
            var state = data.charaState;
            var charID = data.charaID;
            var emojiID = data.emojiID;

            var name = characterFiles.characterList[charID].name;

            if (state == CharacterState.In)
            {
                //�����һ�γ�ʼ����ʱ���In��
                if(data.idx == 0)
                {
                    StartCoroutine(ShowUpAtStart(delay, data));
                }
                else
                {
                    if (charID > 1)
                    {
                        windowsCharacters[curPlace].transform.SetAsLastSibling();
                        windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                        charSortedList.Add(charID);
                        SortWindowsPos();
                        curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);

                        //���ܴ��ڵ�ǰ��ɫIn��ʱ������Ѿ��жԻ�������ˣ���ʱֻ��ҪShowNameʱ�������MoveUpʱ�䣬������һ������Ѿ��жԻ�������ҪMoveDown
                        if (left.curState == CurState.HIDE)
                            left.MoveUp(showNameTime);
                        else if (left.curState == CurState.DOWN)
                            left.MoveUp(showAllTime);

                        if (right.curState == CurState.UP)
                            right.MoveHideName(hideNameTime);

                        left.SetAllDatas(true, charID, name, emojiID);
                        left.SetName(data.name);
                    }
                    else
                    {
                        //ͬ��
                        if (right.curState == CurState.HIDE)
                            right.MoveUp(showNameTime);
                        else if (right.curState == CurState.DOWN)
                            right.MoveUp(showAllTime);

                        if (left.curState == CurState.UP)
                            left.MoveHideName(hideNameTime);

                        right.SetAllDatas(true, charID, name, emojiID);
                        right.SetName(data.name);
                    }
                }
            }
            else if (state == CharacterState.Leave)
            {
                if (right.CharID == charID)
                {
                    if (right.curState == CurState.HIDE)
                        right.MoveDown(hideAllTime - hideNameTime);
                    else if (right.curState == CurState.UP)
                        right.MoveDown(hideAllTime);

                    right.SetAllDatas(true);
                    right.SetName("");
                }
                else if (left.CharID == charID)
                {
                    if (left.curState == CurState.HIDE)
                        left.MoveDown(hideAllTime - hideNameTime);
                    else if (left.curState == CurState.UP)
                        left.MoveDown(hideAllTime);
                    left.SetAllDatas(true);
                    left.SetName("");
                }

                if (charID > 1)
                {
                    var obj = windowsCharacters.Find(x => x.CharID == charID);
                    obj.SetAllDatas(false);
                    charSortedList.Remove(charID);
                    SortWindowsPos();
                }
            }
            else
            {
                if (charID > 1)
                {
                    var c = windowsCharacters.Find(x => x.CharID == charID);
                    if (c)
                    {
                        c.transform.SetAsLastSibling();
                        c.SetAllDatas(true, charID, name, 0);
                    }

                    //���ܻ���ֵ�һ�γ��ֶԻ���ʱ��û��In�����
                    if (left.curState == CurState.HIDE)
                        left.MoveUp(showNameTime);
                    else if (left.curState == CurState.DOWN)
                        left.MoveUp(showAllTime);

                    if (right.curState == CurState.UP)
                        right.MoveHideName(hideNameTime);

                    left.SetAllDatas(true, charID, name, emojiID);
                    left.SetName(data.name);
                }
                else
                {
                    if (right.curState == CurState.HIDE)
                        right.MoveUp(showNameTime);
                    else if (right.curState == CurState.DOWN)
                        right.MoveUp(showAllTime);

                    if (left.curState == CurState.UP)
                        left.MoveHideName(hideNameTime);

                    right.SetAllDatas(true, charID, name, emojiID);
                    right.SetName(data.name);
                }
            }
        }
    }

    private IEnumerator ShowUpAtStart(float time,DiologueData data)
    {
        yield return new WaitForSeconds(time);

        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;

        var name = characterFiles.characterList[charID].name;

        if (charID > 1)
        {
            windowsCharacters[curPlace].transform.SetAsLastSibling();
            windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
            charSortedList.Add(charID);
            SortWindowsPos();
            curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);

            //���ܴ��ڵ�ǰ��ɫIn��ʱ������Ѿ��жԻ�������ˣ���ʱֻ��ҪShowNameʱ�������MoveUpʱ�䣬������һ������Ѿ��жԻ�������ҪMoveDown
            if (left.curState == CurState.HIDE)
                left.MoveUp(showNameTime);
            else if (left.curState == CurState.DOWN)
                left.MoveUp(showAllTime);

            if (right.curState == CurState.UP)
                right.MoveHideName(hideNameTime);

            left.SetAllDatas(true, charID, name, emojiID);
            left.SetName(data.name);
        }
        else
        {
            //ͬ��
            if (right.curState == CurState.HIDE)
                right.MoveUp(showNameTime);
            else if (right.curState == CurState.DOWN)
                right.MoveUp(showAllTime);

            if (left.curState == CurState.UP)
                left.MoveHideName(hideNameTime);

            right.SetAllDatas(true, charID, name, emojiID);
            right.SetName(data.name);
        }

    }

    //��������characterPanel
    private IEnumerator ShowUpAfterCoffee(DiologueData data,float time)
    {
        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;
        var name = characterFiles.characterList[charID].name;

        yield return new WaitForSeconds(time);

        if(charID > 1)
        {
            if (left.CharID != -1)
                left.MoveUp(showAllTime);

            if (right.CharID != -1)
                right.MoveHideName(showAllTime - hideNameTime);

            left.SetAllDatas(true, charID, name,emojiID);
            left.SetName(data.name);
        }
        else
        {
            if (right.CharID != -1)
                right.MoveUp(showAllTime);

            if (left.CharID != -1)
                left.MoveHideName(showAllTime - hideNameTime);

            right.SetAllDatas(true, charID, name,emojiID);
            right.SetName(data.name);
        }


        if (charID > 1)
        {
            if (state == CharacterState.In)
            {
                windowsCharacters[curPlace].transform.SetAsLastSibling();
                windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                charSortedList.Add(charID);
                SortWindowsPos();
                curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);

            }
            else if (state == CharacterState.Leave)
            {
                var obj = windowsCharacters.Find(x => x.CharID == charID);
                obj.SetAllDatas(false);
                charSortedList.Remove(charID);
                SortWindowsPos();
            }
            else
            {
                var c = windowsCharacters.Find(x => x.CharID == charID);
                if (c)
                {
                    c.transform.SetAsLastSibling();
                    c.SetAllDatas(true, charID, name, 0);
                }
            }
        }
    }
    private IEnumerator FoldWhite(float time)
    {
        yield return new WaitForSeconds(time);
        whiteMask.FadeAndReFill(true);
    }

    private void SortWindowsPos()
    {
        var listCount = charSortedList.Count;

        var place = 637 / (listCount + 1);

        for (int i = listCount - 1; i >=0; i--)
        {
            var m = listCount - i;
            windowsCharacters.Find(x => x.CharID == charSortedList[i]).GetComponent<RectTransform>().anchoredPosition = new Vector2(m * place, -365);
        }
    }
}

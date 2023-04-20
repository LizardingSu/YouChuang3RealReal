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
    /// 多种情况所需时间
    /// 从显示名字到不显示名字：hideNameTime
    /// 从不显示名字到显示名字：showNameTime
    /// 从最底部到显示名字：showAllTime
    /// 从显示名字到最底部：hideAllTime
    /// </summary>
    public float hideNameTime = 0.2f;
    public float showNameTime = 0.2f;
    public float showAllTime = 1.0f;
    public float hideAllTime = 1.0f;

    [Tooltip("控制的是重新升起Character框之前的等待时间")]
    public float delay = 0.6f;

    [Tooltip("控制的是进行白罩的一个消的失之前的等待时间")]
    public float whiteFoldDelay = 1.0f;

    private bool isCoffeeBefore = false;

    public List<CharacterEntryController> windowsCharacters = new List<CharacterEntryController>(3);
    public List<int> charSortedList = new List<int>(3);

    public ConfigCharacterFile characterFiles;

    //控制当前空余的窗口
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
   
    //无法处理回环情况,但是没有这种情况
   private void WillChangeCharacter(DiologueData data)
    {
        //同样的，如果后续不是对话就会出错
        if (data.processState == ProcessState.Coffee)
        {
            if(diologueState.state == DiologueState.Normal)
            {
                whiteMask.FadeAndReFill(false);
            }
            isCoffeeBefore = true;
        }
    }

   private void ChangeCharacters(DiologueData data)
    {
        //如果是初始化阶段
        if(data.idx == 0)
        {
            whiteMask.FadeAndReFill(false);
        }

        if (diologueState.state == DiologueState.Normal)
        {
            //如果是做咖啡阶段，收起
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

        if(data.processState == ProcessState.Select||data.processState == ProcessState.Diologue) 
        {
            var state = data.charaState;
            var charID = data.charaID;
            var emojiID = data.emojiID;

            var name = characterFiles.characterList[charID].name;

            if (state == CharacterState.In)
            {
                if (charID > 1)
                {
                    windowsCharacters[curPlace].transform.SetAsLastSibling();
                    windowsCharacters[curPlace].SetAllDatas(true, charID, name, emojiID);
                    charSortedList.Add(charID);
                    SortWindowsPos();
                    curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);

                    //可能存在当前角色In的时候，左侧已经有对话框出现了，此时只需要ShowName时间而不是MoveUp时间，并且另一侧如果已经有对话框，则需要MoveDown
                    if (left.curState == CurState.HIDE)
                        left.MoveUp(showNameTime);
                    else if (left.curState == CurState.DOWN)
                        left.MoveUp(showAllTime);

                    if (right.curState == CurState.UP)
                        right.MoveHideName(hideNameTime);

                    left.SetAllDatas(true, charID, name);
                    left.SetName(data.name);
                }
                else
                {
                    //同理
                    if (right.curState == CurState.HIDE)
                        right.MoveUp(showNameTime);
                    else if (right.curState == CurState.DOWN)
                        right.MoveUp(showAllTime);

                    if (left.curState == CurState.UP)
                        left.MoveHideName(hideNameTime);

                    right.SetAllDatas(true, charID, name);
                    right.SetName(data.name);
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
                        c.SetAllDatas(true, charID, name, emojiID);
                    }

                    //可能会出现第一次出现对话框时还没有In的情况
                    if (left.curState == CurState.HIDE)
                        left.MoveUp(showNameTime);
                    else if (left.curState == CurState.DOWN)
                        left.MoveUp(showAllTime);

                    if (right.curState == CurState.UP)
                        right.MoveHideName(hideNameTime);

                    left.SetAllDatas(true, charID, name);
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

                    right.SetAllDatas(true, charID, name);
                    right.SetName(data.name);
                }
            }
        }
    }

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

            left.SetAllDatas(true, charID, name);
            left.SetName(data.name);
        }
        else
        {
            if (right.CharID != -1)
                right.MoveUp(showAllTime);

            if (left.CharID != -1)
                left.MoveHideName(showAllTime - hideNameTime);

            right.SetAllDatas(true, charID, name);
            right.SetName(data.name);
        }


        if (charID > 1)
        {
            if (state == CharacterState.In)
            {
                windowsCharacters[curPlace].transform.SetAsLastSibling();
                windowsCharacters[curPlace].SetAllDatas(true, charID, name, emojiID);
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
                    c.SetAllDatas(true, charID, name, emojiID);
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

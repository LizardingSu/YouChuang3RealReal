using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class CharacterController : MonoBehaviour
{
    public S_CentralAccessor central;
    private DioLogueState diologueState;

    public CharacterEntryController left;
    public CharacterEntryController right;
    public WhiteAnim whiteMask;

    public Coroutine coroutine;
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
    public float fadeTime = 0.15f;

    [Tooltip("控制的是重新升起Character框之前的等待时间")]
    public float delay = 1.0f;

    [Tooltip("控制的是进行白罩的一个消的失之前的等待时间")]
    public float whiteFoldDelay = 1.0f;

    public bool isCoffeeBefore = false;

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

    public void Clear()
    {
        curPlace = 0;
        charSortedList.Clear();

        foreach (var c in windowsCharacters)
            c.SetAllDatas(false);

        //全部移动下去
        left.MoveDown(0);
        left.SetAllDatas(true);
        right.MoveDown(0);
        right.SetAllDatas(true);

        whiteMask.mask = 0;
        whiteMask.material.SetFloat("_MyMask", whiteMask.mask);
    }

    public void KillAllAnim()
    {
        left.KillAllAnim();
        right.KillAllAnim();
    }

    //无法处理回环情况,但是没有这种情况
    private void WillChangeCharacter(DiologueData data, DiologueData data2)
    {
        //如果后续不是对话就会出错
        if (data.processState == ProcessState.Coffee)
        {
            //如果是正常读的时候，则需要白幕布消失，如果不是则不需要
            if (diologueState.state == DioState.Normal)
            {
                whiteMask.FadeAndReFill(false);
            }
            //根据之前是不是咖啡来判断后续
            isCoffeeBefore = true;
        }
        //如果上次和这次都有人在同侧，且这次有人入场，则播放入场动画
        else if (data.processState == ProcessState.Diologue || data.processState == ProcessState.Select)
        {
            if (data2.processState == ProcessState.Diologue && data2.charaState == CharacterState.In)
            {

                if (diologueState.state != DioState.Normal) return;

                //让人物进场的动画
                if (data2.charaID > 1 && data.charaID > 1)
                {
                    left.SetAllDatas(true, data2.charaID, characterFiles.characterList[data2.charaID].name, data2.emojiID);
                    left.SetName(data2.name);
                    left.FadeAnim(fadeTime);
                }
                else if (data2.charaID <= 1 && data.charaID <= 1)
                {
                    right.SetAllDatas(true, data2.charaID, characterFiles.characterList[data2.charaID].name, data2.emojiID);
                    right.SetName(data2.name);
                    right.FadeAnim(fadeTime);
                }
            }
            else if (data2.processState == ProcessState.Diologue && data2.charaState == CharacterState.Twinkle)
            {
                if (diologueState.state != DioState.Normal) return;

                if (data2.charaID > 1)
                {
                    left.Twinkle();
                }
                else
                {
                    right.Twinkle();
                }
            }
        }
    }

    private void ChangeCharacters(DiologueData data)
    {
        //如果是初始化阶段
        if (data.idx == 0)
        {
            //展开白色幕布
            whiteMask.FadeAndReFill(false);
        }

        //如果当前是coffee情况
        if (diologueState.state == DioState.Normal)
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

                //收起白色幕布
                StartCoroutine(FoldWhite(whiteFoldDelay));

                Debug.Log(data.idx + "  " + "Left" + left.curState + "  " + "Right" + right.curState);
                return;
            }

            //如果之前是做咖啡界面，则重新升起所有界面
            if (isCoffeeBefore)
            {
                isCoffeeBefore = false;
                StartCoroutine(ShowUpAfterCoffee(data, delay));
                return;
            }
        }
        else
        {
            //如果是Auto的话，则直接结束咖啡
            if (isCoffeeBefore) isCoffeeBefore = false;
        }

        //如果是对话或者选择阶段才进入
        if (data.processState == ProcessState.Select || data.processState == ProcessState.Diologue)
        {
            var state = data.charaState;
            var charID = data.charaID;
            var emojiID = data.emojiID;

            if (charID == -1 && emojiID == -1 && data.processState == ProcessState.Diologue)
            {
                //Debug.Log(data.idx + "  " + "Left" + left.curState + "  " + "Right" + right.curState);
                Narration();
                return;
            }

            var name = characterFiles.characterList[charID].name;

            //如果有人进入场景
            if (state == CharacterState.In)
            {
                //如果第一次初始化的时候就In了
                if (data.idx == 0)
                {
                    //协程，你是个傻呗！
                    if (diologueState.state == DioState.Auto)
                        ShowUpAtStartImee(data);
                    else
                        StartCoroutine(ShowUpAtStart(delay, data));
                }
                else
                {
                    if (charID > 1)
                    {
                        //窗口的情况
                        bool isIn = false;


                        foreach (var windowsCharacter in windowsCharacters)
                        {
                            if (charID == windowsCharacter.CharID)
                            {
                                isIn = true;
                                break;
                            }
                        }
                        if ((!isIn) && IsWindow(charID))
                        {
                            windowsCharacters[curPlace].transform.SetAsLastSibling();
                            windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                            charSortedList.Add(charID);
                            SortWindowsPos();
                            curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);
                        }

                        //可能存在当前角色In的时候，左侧已经有对话框出现了，此时只需要ShowName时间而不是MoveUp时间，并且另一侧如果已经有对话框，则需要MoveDown
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
                        //同理
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
                    if (IsWindow(charID))
                    {
                        var obj = windowsCharacters.Find(x => x.CharID == charID);
                        obj.SetAllDatas(false);
                        charSortedList.Remove(charID);
                        SortWindowsPos();
                    }
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

                    //可能会出现第一次出现对话框时还没有In的情况
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

            //更改颜色
            ChangeColor();

            //立绘动画
            AnimationOfCharacter(data);
        }
    }


    private void ShowUpAtStartImee(DiologueData data)
    {
        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;

        var name = characterFiles.characterList[charID].name;

        if (charID > 1)
        {
            //对于开头，不可能出现一个角色已经In过的情况
            if (IsWindow(charID))
            {
                windowsCharacters[curPlace].transform.SetAsLastSibling();
                windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                charSortedList.Add(charID);
                SortWindowsPos();
                curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);
            }

            //可能存在当前角色In的时候，左侧已经有对话框出现了，此时只需要ShowName时间而不是MoveUp时间，并且另一侧如果已经有对话框，则需要MoveDown
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
            //同理
            if (right.curState == CurState.HIDE)
                right.MoveUp(showNameTime);
            else if (right.curState == CurState.DOWN)
                right.MoveUp(showAllTime);

            if (left.curState == CurState.UP)
                left.MoveHideName(hideNameTime);

            right.SetAllDatas(true, charID, name, emojiID);
            right.SetName(data.name);
        }

        AnimationOfCharacter(data);
    }

    private IEnumerator ShowUpAtStart(float time, DiologueData data)
    {
        yield return new WaitForSeconds(time);

        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;

        var name = characterFiles.characterList[charID].name;

        if (charID > 1)
        {
            //对于开头，不可能出现一个角色已经In过的情况
            if (IsWindow(charID))
            {
                windowsCharacters[curPlace].transform.SetAsLastSibling();
                windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                charSortedList.Add(charID);
                SortWindowsPos();
                curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);
            }

            //可能存在当前角色In的时候，左侧已经有对话框出现了，此时只需要ShowName时间而不是MoveUp时间，并且另一侧如果已经有对话框，则需要MoveDown
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
            //同理
            if (right.curState == CurState.HIDE)
                right.MoveUp(showNameTime);
            else if (right.curState == CurState.DOWN)
                right.MoveUp(showAllTime);

            if (left.curState == CurState.UP)
                left.MoveHideName(hideNameTime);

            right.SetAllDatas(true, charID, name, emojiID);
            right.SetName(data.name);
        }

        AnimationOfCharacter(data);
    }

    //升起所有characterPanel
    private IEnumerator ShowUpAfterCoffee(DiologueData data, float time)
    {
        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;

        yield return new WaitForSeconds(time);

        if (charID == -1 && emojiID == -1)
        {
            Narration();
        }
        else
        {
            var name = characterFiles.characterList[charID].name;

            //窗边的人物显示
            if (charID > 1)
            {
                if (state == CharacterState.In)
                {
                    bool isIn = false;
                    foreach (var windowsCharacter in windowsCharacters)
                    {
                        if (charID == windowsCharacter.CharID)
                        {
                            isIn = true;
                            break;
                        }
                    }

                    if (!isIn && IsWindow(charID))
                    {
                        windowsCharacters[curPlace].transform.SetAsLastSibling();
                        windowsCharacters[curPlace].SetAllDatas(true, charID, name, 0);
                        charSortedList.Add(charID);
                        SortWindowsPos();
                        curPlace = windowsCharacters.FindIndex(x => x.CharID == -1);
                    }

                }
                else if (state == CharacterState.Leave)
                {
                    if (IsWindow(charID))
                    {
                        var obj = windowsCharacters.Find(x => x.CharID == charID);
                        obj.SetAllDatas(false);
                        charSortedList.Remove(charID);
                        SortWindowsPos();
                    }
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

            //左右角色的显示界面
            if (charID > 1)
            {
                if (left.CharID != -1)
                    left.MoveUp(showAllTime);

                if (right.CharID != -1)
                    right.MoveHideName(showAllTime - hideNameTime);

                left.SetAllDatas(true, charID, name, emojiID);
                left.SetName(data.name);
            }
            else
            {
                if (right.CharID != -1)
                    right.MoveUp(showAllTime);

                if (left.CharID != -1)
                    left.MoveHideName(showAllTime - hideNameTime);

                right.SetAllDatas(true, charID, name, emojiID);
                right.SetName(data.name);
            }
            //改变颜色
            ChangeColor();
        }

        AnimationOfCharacter(data);
    }

    private void Narration()
    {

        if (left.curState == CurState.UP)
            left.MoveHideName(hideNameTime);

        if (right.curState == CurState.UP)
            right.MoveHideName(hideNameTime);

        if (diologueState.date == 0)
        {
            return;
        }

        left.image.color = new Color(0.5f, 0.5f, 0.5f, 1);
        right.image.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
    private void ChangeColor()
    {
        Debug.Log(diologueState.curData.idx);

        //如果本身没立绘，则不需要变色
        if (left.image.color != new Color(1, 1, 1, 0))
            left.image.color = new Color(1, 1, 1, 1);
        if (right.image.color != new Color(1, 1, 1, 0))
            right.image.color = new Color(1, 1, 1, 1);

        if (left.curState == CurState.HIDE && right.curState == CurState.UP && left.image.color != new Color(1, 1, 1, 0))
        {
            left.image.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        else if (left.curState == CurState.UP && right.curState == CurState.HIDE && right.image.color != new Color(1, 1, 1, 0))
        {
            right.image.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }
    private void AnimationOfCharacter(DiologueData data)
    {
        if (data.charaState == CharacterState.None || data.charaState == CharacterState.In || data.charaState == CharacterState.Leave)
            return;

        if (diologueState.state == DioState.Auto)
            return;

        var charaEntry = data.charaID > 1?left:right;

        switch (data.charaState)
        {
            case CharacterState.Shake:
                charaEntry.Shake();
                break;
            case CharacterState.Tremble:
                charaEntry.Tremble();
                break;
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

    private bool IsWindow(int charID)
    {
        return charID != 7 && charID != 8 && charID != 9;
    }
}

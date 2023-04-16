using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterController : MonoBehaviour
{
    public S_CentralAccessor m_central;
    private DioLogueState m_diologueState;

    public CharacterEntryController DanXi;
    public CharacterEntryController ChuanShu;
    public List<CharacterEntryController> WindowsCharacters = new List<CharacterEntryController>(3);
    public List<int> charSortedList = new List<int>(3);

    public ConfigCharacterFile characterFiles;

    //控制当前空余的窗口
    private int curPlace = 0;

    private void Awake()
    {
        DanXi.CharID = -1;
        DanXi.ImageID = -1;
        DanXi.gameObject.SetActive(false);

        ChuanShu.CharID = -1;
        ChuanShu.ImageID = -1;
        ChuanShu.gameObject.SetActive(false);

        for(int i = 0; i < WindowsCharacters.Count; i++)
        {
            WindowsCharacters[i].CharID = -1;
            WindowsCharacters[i].ImageID = -1;
            WindowsCharacters[i].gameObject.SetActive(false);
        }

        m_diologueState = m_central._DioLogueState;
        m_diologueState.dialogueWillChange.AddListener(WillChangeCharacter);
        m_diologueState.dialogueChanged.AddListener(ChangeCharacters);
    }

    private void OnDestroy()
    {
        m_diologueState.dialogueWillChange.RemoveListener(WillChangeCharacter);
        m_diologueState.dialogueChanged.RemoveListener(ChangeCharacters);
    }
   
    //无法处理回环情况
   private void WillChangeCharacter(DiologueData data)
    {
        if(data.processState == ProcessState.Select)
        {
            if (data.charaID == 0)
            {
                if (ChuanShu.gameObject.activeSelf)
                    ChuanShu.SetAllDatas(true, 0, "传书", data.emojiID);
            }
            else if (data.charaID == 1)
            {
                if (DanXi.gameObject.activeSelf)
                    DanXi.SetAllDatas(true, 1, "旦夕", data.emojiID);
            }
        }
    }
   private void ChangeCharacters(DiologueData data)
    {
        if (data.processState == ProcessState.Coffee) return;


        var state = data.charaState;
        var charID = data.charaID;
        var emojiID = data.emojiID;

        var name = characterFiles.characterList[charID].name;

        if (state == CharacterState.In)
        {
            if(charID == 0)
            {
                ChuanShu.SetAllDatas(true, charID,name, emojiID);
            }
            else if(charID == 1)
            {
                DanXi.SetAllDatas(true, charID, name,emojiID);
            }
            else
            {
                WindowsCharacters[curPlace].transform.SetAsLastSibling();
                WindowsCharacters[curPlace].SetAllDatas(true, charID, name, emojiID);

                charSortedList.Add(charID);

                SortWindowsPos();
                curPlace = WindowsCharacters.FindIndex(x => x.CharID == -1);
            }
        }
        else if(state == CharacterState.Leave)
        {
            if (charID == 0)
            {
                ChuanShu.SetAllDatas(false, 0,"传书", -1);
            }
            else if (charID == 1)
            {
                DanXi.SetAllDatas(false,1,"旦夕",-1);
            }
            else
            {
                var obj = WindowsCharacters.Find(x => x.CharID == charID);
                obj.SetAllDatas(false);

                charSortedList.Remove(charID);

                SortWindowsPos();
            }

        }
        else
        {
            if (charID == 0)
            {
                if (ChuanShu.gameObject.activeSelf)
                    ChuanShu.SetAllDatas(true, 0, "传书", emojiID);
            }
            else if (charID == 1)
            {
                if (DanXi.gameObject.activeSelf)
                    DanXi.SetAllDatas(true, 1, "旦夕", emojiID);
            }
            else
            {
                var c = WindowsCharacters.Find(x => x.CharID == charID);
                if (c)
                {
                    c.transform.SetAsLastSibling();
                    c.SetAllDatas(true, charID, name, emojiID);
                }
            }
        }
            
    }


    private void SortWindowsPos()
    {
        var listCount = charSortedList.Count;

        var place = 637 / (listCount + 1);

        for (int i = listCount - 1; i >=0; i--)
        {
            var m = listCount - i;
            WindowsCharacters.Find(x => x.CharID == charSortedList[i]).GetComponent<RectTransform>().anchoredPosition = new Vector2(m * place, -365);
        }
    }
}

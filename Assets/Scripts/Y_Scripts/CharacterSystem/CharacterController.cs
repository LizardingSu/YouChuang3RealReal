using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterController : MonoBehaviour
{
    public S_CentralAccessor m_central;
    private DioLogueState m_diologueState;

    /// <summary>
    /// 收到最下方为 -1000
    /// 收到下面为 -80
    /// 正常为0
    /// </summary>
    public CharacterEntryController Left;
    public CharacterEntryController Right;

    public List<CharacterEntryController> WindowsCharacters = new List<CharacterEntryController>(3);
    public List<int> charSortedList = new List<int>(3);

    public ConfigCharacterFile characterFiles;

    //控制当前空余的窗口
    private int curPlace = 0;


    private void Awake()
    {
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
   
    //无法处理回环情况,但是没有这种情况
   private void WillChangeCharacter(DiologueData data)
    {
       
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
            if(charID <= 1)
            {
                
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
            if (charID <= 1)
            {
                
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
            if (charID <= 1)
            {
               
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

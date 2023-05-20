using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    public Button button;
    public TMP_Text Detail;
    public SingleInput singleInput;

    public Dictionary<string,uint> correctAndNextIdx = new Dictionary<string, uint>();
    public int maxcharNum = 0;
    public uint Idx;

    private DioLogueState m_dioState;
    private S_ProcessManager m_processManager;

    public bool FirstTime = true;


    private void Awake()
    {
        m_dioState = GameObject.Find("MainManager").GetComponent<DioLogueState>();
        m_processManager = GameObject.Find("MainManager").GetComponent<S_ProcessManager>();

        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
    public void OnClick()
    {
        singleInput.inputField.ActivateInputField();

        //如果是第一次点击，则固定不会触发下一句对话
        if (FirstTime)
        {
            FirstTime = false;
            return;
        }

        foreach (KeyValuePair<string,uint> kvp in correctAndNextIdx)
        {
            if(singleInput.textBox.textInfo.characterCount <= maxcharNum && singleInput.textBox.text.ToLower() == kvp.Key.ToLower())
            {
                m_dioState.OnSelectionSelect(Idx, kvp.Value, kvp.Key);
                return;
            }       
        }
    }

    public void Init(string text,string head,uint Idx,bool isSelectable)
    {
        float charSize = 22;
        if (isSelectable)
        {
            charSize = 30;
        }

        this.Idx = Idx;
        var ta = text.Split('@');

        Detail.fontSize = charSize;
        Detail.text = head + ta[0];

        string logLast = ta[ta.Length - 1];

        for(int i = 1;i<ta.Length - 1; i++)
        {
            var option = ta[i].Split('-');
            correctAndNextIdx.Add(option[0], uint.Parse(option[1]));
            maxcharNum = maxcharNum >= option[0].Count()?maxcharNum : option[0].Count();
        }

        var space = "<size="+Convert.ToString(charSize*1.25)+">";
        for (int i = 0; i < maxcharNum; i++)
        {
            space += "_";
        }
        Detail.text += space + "</size>"+ logLast;

        StartCoroutine(InitInputField(charSize));

        //如果是右侧panel，则可以点，如果是左侧则不可以
        if (!isSelectable) button.interactable = false;
        else button.interactable = true;

        //如果存档里发现已经填过词了，则会自动填上去
        foreach(var choice in m_processManager.m_Saving1.Choices)
        {
            var idx = choice.ID % 1000;
            var day = (choice.ID - idx) / 1000;
            if(day == m_dioState.date)
            {
                if(idx == Idx)
                {
                    if (choice.Answer != "")
                    {
                        singleInput.inputField.text = choice.Answer;
                        return;
                    }
                }
            }
        }
    }

    public void SetButtonImage(Image image)
    {
        button.image = image;
    }

    private IEnumerator InitInputField(float charSize)
    {
        yield return null;
        var pos = GetTextPos();
        //往上调一点
        pos.y += charSize*0.25f;
        singleInput.Init((uint)charSize, (uint)(charSize*1.25), pos, maxcharNum);

    }

    private Vector2 GetTextPos()
    {

        var characterInfos = Detail.textInfo.characterInfo;
        var lineInfos = Detail.textInfo.lineInfo;

        int charaIdx = 0;
        for (int i = 0; i < characterInfos.Length; i++)
        {
            if (characterInfos[i].character == '_')
            {
                charaIdx = i;
                break;
            }
        }

        int lineIdx = characterInfos[charaIdx].lineNumber;
        var totalHeight = 0.0f;
        for(int i = 0; i <= lineIdx; i++)
        {
            totalHeight += lineInfos[i].lineHeight;
        }

        var lineFirstCharIdx = lineInfos[lineIdx].firstCharacterIndex;
        float totalWidth = (characterInfos[charaIdx].bottomLeft - characterInfos[lineFirstCharIdx].bottomLeft).x;

        Vector2 pos = new Vector2(totalWidth, -totalHeight);

        return pos;
    }
}

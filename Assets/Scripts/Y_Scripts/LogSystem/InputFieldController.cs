using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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


    private void Awake()
    {
        m_dioState = GameObject.Find("MainManager").GetComponent<DioLogueState>();

        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
    public void OnClick()
    {
        singleInput.inputField.ActivateInputField();

        foreach (KeyValuePair<string,uint> kvp in correctAndNextIdx)
        {
            if(singleInput.textBox.textInfo.characterCount <= maxcharNum && singleInput.textBox.text == kvp.Key)
            {
                m_dioState.OnSelectionSelect(Idx, kvp.Value);
                return;
            }       
        }
    }

    private void Update()
    {
    }
    public void Init(string text,string head,uint Idx,uint nextIdx,bool isSelectable)
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

        if (!isSelectable) button.interactable = false;
        else button.interactable = true;
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

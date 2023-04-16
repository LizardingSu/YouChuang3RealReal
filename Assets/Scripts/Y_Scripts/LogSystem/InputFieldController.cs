using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Windows.WebCam.VideoCapture;

public class InputFieldController : MonoBehaviour
{
    public Button button;
    public TMP_Text Detail;
    public SingleInput singleInput;

    public string correct;
    public int charNum;
    public uint Idx;
    public uint nextIdx;

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
        if (singleInput.textBox.textInfo.characterCount != charNum || singleInput.textBox.text != correct)
        {
            singleInput.inputField.ActivateInputField();
            return;
        }

        m_dioState.OnSelectionSelect(Idx, nextIdx);
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
        this.nextIdx = nextIdx;
        var ta = text.Split('@');

        Detail.fontSize = charSize;
        Detail.text = head + ta[0];

        correct = ta[1];
        charNum = ta[1].Length;

        var space = "<size="+Convert.ToString(charSize*1.25)+">";
        for (int i = 0; i < charNum; i++)
        {
            space += "_";
        }
        Detail.text += space + "</size>"+ ta[2];

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
        singleInput.Init((uint)charSize, (uint)(charSize*1.25), pos, charNum);

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

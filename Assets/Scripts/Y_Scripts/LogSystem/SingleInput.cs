using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class SingleInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text textBox;
    public RawImage image;

    public RectTransform m_rectTransform;
    public RectTransform m_inputRect;
    public RectTransform textBoxRect;

    private void Awake()
    {
    }

    public void Init(uint pointSize,uint largerSize,Vector2 pos ,int charNum)
    {
        textBox.fontSize = pointSize;
        if(pointSize == 30)
        {
            textBox.characterSpacing = 20;
        }
        else
        {
            textBox.characterSpacing = 4;
        }
        inputField.characterLimit = charNum;
        inputField.ActivateInputField();

        //image.uvRect = new Rect(1, 1, charNum, 1);

        m_inputRect.sizeDelta = new Vector2((pointSize + 1) * charNum, largerSize);
        textBoxRect.sizeDelta = new Vector2((pointSize) * charNum, pointSize);

        m_rectTransform.anchoredPosition = pos;
    }

    public void Update()
    {
        textBox.text = inputField.text;
    }


}

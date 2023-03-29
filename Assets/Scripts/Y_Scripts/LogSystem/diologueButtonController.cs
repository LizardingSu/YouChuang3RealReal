using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class diologueButtonController : MonoBehaviour
{
    private TMP_Text textBox;
    private Button button;

    private uint Idx;
    private uint nextIdx;

    private DioLogueState m_dioState;

    public void Awake()
    {
        button = GetComponent<Button>();
        textBox = GetComponent<TMP_Text>();

        m_dioState = GameObject.Find("MainManager").GetComponent<DioLogueState>();

        button.onClick.AddListener(OnClick);
    }

    public void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void Init(string text,uint idx,uint nextIdx)
    {
        Idx = idx;
        textBox.text = text;
        this.nextIdx = nextIdx;
    }

    public void OnClick()
    {
        m_dioState.OnSelectionSelect(Idx, nextIdx);
    }
}

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
    private S_AudioManager m_audioManager;

    public AudioClip a;

    public void Awake()
    {
        button = GetComponent<Button>();
        textBox = GetComponent<TMP_Text>();

        var m = GameObject.Find("MainManager");
        m_dioState = m.GetComponent<DioLogueState>();
        m_audioManager = m.GetComponent<S_AudioManager>();


        button.onClick.AddListener(OnClick);
    }

    public void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void Init(string text, uint idx, uint nextIdx, bool isSelectable)
    {
        Idx = idx;
        textBox.text = text;
        this.nextIdx = nextIdx;

        //������Ҳ࣬����Ե㣬����򲻿���
        if (!isSelectable)
        {
            button.interactable = false;
            textBox.fontSize = 22;
        }
        else
        {
            button.interactable = true;
            textBox.fontSize = 30;
        }
    }

    public void SetButtonImage(Image image)
    {
        button.image = image;
    }

    public void OnClick()
    {
        m_audioManager.PlaySE(a);

        m_dioState.OnSelectionSelect(Idx, nextIdx);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LogEntryController : MonoBehaviour
{
    public LogEntry m_logEntry;

    //text
    public TMP_Text m_name;
    public TMP_Text m_diogue;

    //selectButton
    private Button m_dioBtn;
    private Button m_inputBtn;

    private Image m_dioBox;
    private VerticalLayoutGroup m_layoutGroup;

    private void Awake()
    {
        m_name = transform.Find("BC/Name").GetComponent<TMP_Text>();
        m_diogue = transform.Find("BC/Select/Dio").GetComponent<TMP_Text>();

        m_dioBtn = transform.Find("BC/Select").GetComponent<Button>();
        m_inputBtn = transform.Find("BC/TextInput").GetComponent <Button>();
        
        //BC
        m_dioBox = GetComponentInChildren<Image>();
        m_layoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    public void Init(int idx,LogEntry logEntry,bool isFirstTime)
    {
        var c_LayoutGroup = m_dioBox.gameObject.GetComponent<VerticalLayoutGroup>();

        var obverse = new Vector3(1, 1, 1);
        var reverse = new Vector3(-1, 1, 1);

        m_name.text = logEntry.Name;
        m_diogue.text = logEntry.Log;

        if (logEntry.Left)
        {
            m_layoutGroup.childAlignment = TextAnchor.UpperLeft;
            m_layoutGroup.padding.left = -30;
            c_LayoutGroup.childAlignment = TextAnchor.UpperLeft;

            m_dioBox.transform.localScale = obverse;
            m_name.transform.localScale = obverse;
            m_dioBtn.transform.localScale = obverse;
            m_inputBtn.transform.localScale = obverse;
        }
        else
        {
            m_layoutGroup.childAlignment = TextAnchor.UpperRight;
            m_layoutGroup.padding.left = 30;
            c_LayoutGroup.childAlignment = TextAnchor.UpperRight;

            //整体翻转
            m_dioBox.transform.localScale = reverse;
            //BC的内容物
            m_name.transform.localScale = reverse;
            m_dioBtn.transform.localScale = reverse;
            m_inputBtn.transform.localScale = reverse;
        }

        if(logEntry.Left)
        {
            //hardcode
            c_LayoutGroup.padding = new RectOffset(40, 10, 15, 15);

            m_name.gameObject.SetActive(true);
            m_dioBtn.enabled = false;
            m_dioBtn.gameObject.GetComponent<Image>().enabled = false;
            m_inputBtn.gameObject.SetActive(false);
        }
        else
        {
            //hardcode
            c_LayoutGroup.padding = new RectOffset(60, 30, 30, 30);
            c_LayoutGroup.spacing = 15;

            m_name.gameObject.SetActive(false);
            m_dioBtn.enabled = true;
            m_dioBtn.gameObject.GetComponent<Image>().enabled = true;
            m_inputBtn.gameObject.SetActive(true);
        }
    }
}

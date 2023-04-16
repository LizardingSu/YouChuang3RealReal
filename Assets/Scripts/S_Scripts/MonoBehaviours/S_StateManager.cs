using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public enum PlaySceneState
{
    Setting,
    Log,
    Calendar,
    Note
}

public class S_StateManager : MonoBehaviour
{
    public PlaySceneState State { get; private set; } = PlaySceneState.Setting;

    //�ĸ�panel��Ӧ��gameobject
    public GameObject SettingPanel;
    public GameObject LogPanel;
    public GameObject CalendarPanel;
    public GameObject NotePanel;

    //�Ҳ�����
    //public GameObject RightArea;

    ////�Ҳ�Note����
    //public GameObject NoteScene;

    //SYJ��demo������ʹ�õ�ͼƬ
    public Sprite DefaultScene;
    public Sprite NotePanelScene;

    //�л�״̬ʱ���ȵ�����Ϸ�����е�ǰ״̬��Ӧ��״̬ȡ������
    #region ״̬ȡ������
    public void CancelStateSetting()
    {
        SettingPanel.SetActive(false);
    }

    public void CancelStateLog()
    {
        LogPanel.SetActive(false);
        GameObject.Find("MainManager").GetComponent<DioLogueState>().SetButtonsActive(false);
    }

    public void CancelStateCalendar()
    {
        List<GameObject> nodeList = CalendarPanel.GetComponent<S_CalendarPanelManager>().currentNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeList[i].SetActive(false);
        }
        
        CalendarPanel.SetActive(false);
    }

    public void CancelStateNote()
    {
        if (NotePanel.GetComponent<S_NotePanelManager>().CurrentActiveItem != null)
        {
            NotePanel.GetComponent<S_NotePanelManager>().CurrentActiveItem.GetComponent<Image>().material = null;
            NotePanel.GetComponent<S_NotePanelManager>().CurrentActiveItem = null;
        }
        else
        {
            NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("BlackMask").gameObject.SetActive(false);
            NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("DialogBoxes").gameObject.SetActive(false);
        }
        NotePanel.SetActive(false);
        NotePanel.transform.Find("InfoBox").gameObject.SetActive(false);
    }
    #endregion

    //״̬ȡ������������Ϻ��ٵ���״̬�л�����
    #region ״̬�л�����
    public void StateSwitchToSetting()
    {
        SettingPanel.SetActive(true);
        State = PlaySceneState.Setting;
    }

    public void StateSwitchToLog()
    {
        LogPanel.SetActive(true);
        State = PlaySceneState.Log;
        GameObject.Find("MainManager").GetComponent<DioLogueState>().SetButtonsActive(true);
    }

    public void StateSwitchToCalendar()
    {
        CalendarPanel.SetActive(true);
        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons(9);     //�������ݣ�֮�����ݴ浵���г�ʼ��
        State = PlaySceneState.Calendar;
    }

    public void StateSwitchToNote()
    {
        NotePanel.SetActive(true);
        NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("BlackMask").gameObject.SetActive(true);
        NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("DialogBoxes").gameObject.SetActive(true);
        NotePanel.transform.Find("DefaultText").gameObject.SetActive(true);
        //NoteScene.SetActive(true);
        State = PlaySceneState.Note;
    }
    #endregion

    //����Stateѡ���Ӧ��Cancel�������÷�����SwitchTo����֮ǰ����
    public void CancelCurrentState()
    {
        switch (State)
        {
            case PlaySceneState.Setting:
                CancelStateSetting();
                break;
                case PlaySceneState.Log:
                CancelStateLog();
                break;
                case PlaySceneState.Calendar:
                CancelStateCalendar();
                break;
                case PlaySceneState.Note:
                CancelStateNote();
                break;
        }
    }
}

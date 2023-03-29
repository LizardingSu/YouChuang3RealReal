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
    public GameObject RightArea;

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
        
    }

    public void CancelStateCalendar()
    {
        CalendarPanel.SetActive(false);
    }

    public void CancelStateNote()
    {
        NotePanel.SetActive(false);
        RightArea.GetComponent<Image>().sprite = DefaultScene;
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
        
        State = PlaySceneState.Log;
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
        RightArea.GetComponent<Image>().sprite = NotePanelScene;
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

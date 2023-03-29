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

    //四个panel对应的gameobject
    public GameObject SettingPanel;
    public GameObject LogPanel;
    public GameObject CalendarPanel;
    public GameObject NotePanel;

    //右侧区域
    public GameObject RightArea;

    //SYJ于demo测试中使用的图片
    public Sprite DefaultScene;
    public Sprite NotePanelScene;

    //切换状态时首先调用游戏场景中当前状态对应的状态取消函数
    #region 状态取消函数
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

    //状态取消函数调用完毕后再调用状态切换函数
    #region 状态切换函数
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
        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons(9);     //测试数据，之后会根据存档进行初始化
        State = PlaySceneState.Calendar;
    }

    public void StateSwitchToNote()
    {
        NotePanel.SetActive(true);
        RightArea.GetComponent<Image>().sprite = NotePanelScene;
        State = PlaySceneState.Note;
    }
    #endregion

    //根据State选择对应的Cancel方法，该方法于SwitchTo函数之前调用
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

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
    //public GameObject RightArea;

    ////右侧Note界面
    //public GameObject NoteScene;

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

    //状态取消函数调用完毕后再调用状态切换函数
    #region 状态切换函数
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
        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons(9);     //测试数据，之后会根据存档进行初始化
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

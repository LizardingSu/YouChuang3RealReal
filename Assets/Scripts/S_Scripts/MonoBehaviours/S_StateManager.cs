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
    public PlaySceneState State { get; private set; } = PlaySceneState.Log;

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
    //public Sprite DefaultScene;
    //public Sprite NotePanelScene;

    public GameObject MainScene;
    public GameObject NoteScene;

    //不同天数的信息界面标签素材

    public Sprite DialogBox1to4;
    public Sprite DialogBox5to8;

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

        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons();
        CalendarPanel.GetComponent<S_CalendarPanelManager>().currentActiveDayButton = null;

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
        MainScene.SetActive(true);
        NoteScene.SetActive(false);
    }
    #endregion

    //状态取消函数调用完毕后再调用状态切换函数
    #region 状态切换函数
    public void StateSwitchToSetting()
    {
        SettingPanel.SetActive(true);
        State = PlaySceneState.Setting;
        //GetComponent<S_CentralAccessor>().ProcessManager.LoadProfile();
    }

    public void StateSwitchToLog()
    {
        LogPanel.SetActive(true);
        State = PlaySceneState.Log;
        if (GameObject.Find("MainManager").GetComponent<DioLogueState>().centralAccessor.GameManager.GamePlaying == false)
        {
            GameObject.Find("MainManager").GetComponent<DioLogueState>().SetButtonsActive(true);
        }
    }

    public void StateSwitchToCalendar()
    {
        CalendarPanel.SetActive(true);
        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitAllDays();
        CalendarPanel.GetComponent<S_CalendarPanelManager>().InitDayButtons();
        State = PlaySceneState.Calendar;
    }

    public void StateSwitchToNote()
    {
        MainScene.SetActive(false);
        NoteScene.SetActive(true);
        NotePanel.SetActive(true);
        NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("BlackMask").gameObject.SetActive(true);

        GameObject db = NotePanel.GetComponent<S_NotePanelManager>().NoteScene.transform.Find("DialogBoxes").gameObject;
        db.SetActive(true);

        GameObject buttons = NoteScene.transform.Find("Buttons").gameObject;
        buttons.SetActive(true);

        NotePanel.transform.Find("DefaultText").gameObject.SetActive(true);
        //NoteScene.SetActive(true);
        State = PlaySceneState.Note;

        //根据天数改变某些东西
        int curDate = (int)GetComponent<S_CentralAccessor>()._DioLogueState.date;

        if (curDate < 5)
        {
            db.GetComponent<Image>().sprite = DialogBox1to4;
            db.transform.Find("柜子").gameObject.SetActive(false);
            buttons.transform.Find("柜子").gameObject.SetActive(false);

        }
        else if (curDate > 4 && curDate < 9)
        {
            db.GetComponent<Image>().sprite = DialogBox5to8;
            db.transform.Find("柜子").gameObject.SetActive(true);
            buttons.transform.Find("柜子").gameObject.SetActive(true);
        }
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

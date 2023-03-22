using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //切换状态时首先调用游戏场景中当前状态对应的状态取消函数
    #region 状态取消函数
    public void CancelStateSetting()
    {

    }

    public void CancelStateLog()
    {

    }

    public void CancelStateCalendar()
    {

    }

    public void CancelStateNote()
    {

    }
    #endregion

    //状态取消函数调用完毕后再调用状态切换函数
    #region 状态切换函数
    public void StateSwitchToSetting()
    {

        State = PlaySceneState.Setting;
    }

    public void StateSwitchToLog()
    {

        State = PlaySceneState.Log;
    }

    public void StateSwitchToCalendar()
    {

        State = PlaySceneState.Calendar;
    }

    public void StateSwitchToNote()
    {

        State = PlaySceneState.Note;
    }
    #endregion
}

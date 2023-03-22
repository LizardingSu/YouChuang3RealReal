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

    //�л�״̬ʱ���ȵ�����Ϸ�����е�ǰ״̬��Ӧ��״̬ȡ������
    #region ״̬ȡ������
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

    //״̬ȡ������������Ϻ��ٵ���״̬�л�����
    #region ״̬�л�����
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class StarterController : MonoBehaviour
{
    public Dictionary<string, Image> maskList = new Dictionary<string, Image>();
    public DioLogueState dioState;
    public S_ProcessManager pm;
    public S_StateManager sm;

    private void Awake()
    {
        //����mask��dictionary�
        foreach(Transform child in this.transform)
        {
            maskList.Add(child.gameObject.name,child.gameObject.GetComponent<Image>());
            child.gameObject.SetActive(false);
        }

        //ע�����¼�
        InitClickEvent();
        dioState.dialogueChanged.AddListener(OnStarterDiologue);
    }

    private void OnDestroy()
    {
        //ȥ������¼�
        RemoveClickEvent();
        dioState.dialogueChanged.RemoveListener(OnStarterDiologue);

        maskList.Clear();
    }

    private void InitClickEvent()
    {
        maskList["yellow"].GetComponent<Button>().onClick.AddListener(onClickYellow);
        maskList["left"].GetComponent<Button>().onClick.AddListener(onClickLeft);
        maskList["right"].GetComponent<Button>().onClick.AddListener(onClickRight);
        maskList["rightPanel"].GetComponent<Button>().onClick.AddListener(onClickRightPanel);
    }

    private void RemoveClickEvent()
    {
        maskList["yellow"].GetComponent<Button>().onClick.RemoveListener(onClickYellow);
        maskList["left"].GetComponent<Button>().onClick.RemoveListener(onClickLeft);
        maskList["right"].GetComponent<Button>().onClick.RemoveListener(onClickRight);
        maskList["rightPanel"].GetComponent<Button>().onClick.RemoveListener(onClickRightPanel);
    }

    public void OnStarterDiologue(DiologueData data)
    {
        if (!IsFirstTime(data)||dioState.state == DioState.Auto)
            return;

        StartCoroutine(delayForShowMask(data));
    }

    private IEnumerator delayForShowMask(DiologueData data)
    {
        dioState.SetButtonsActive(false);

        yield return new WaitForSeconds(0.4f);

        if(data.idx == 1)
        {
            var m = maskList["left"];
            m.gameObject.SetActive(true);
            var t = m.GetComponentsInChildren<TMP_Text>(true);
            Debug.Log(t.Count());
            t[0].gameObject.SetActive(true);
            t[1].gameObject.SetActive(false);
            t[2].gameObject.SetActive(false);
        }
        if(data.idx == 38)
        {
            var m = maskList["yellow"];
            m.gameObject.SetActive(true);
        }

    }

    private bool IsFirstTime(DiologueData data)
    {
        if (data.idx != 1 && data.idx != 38)
            return false;

        //���idxΪ0�������û���κζ���浵
        if (data.idx == 1 && pm.m_Saving1.Choices.Count <= 1)
        {
            return true;
        }
        //���idxΪ38����û�еڶ���Ĵ浵����û����
        else if(data.idx == 38 && pm.m_Saving1.Choices.Find(c => (int)(c.ID / 1000) >= 2) == null)
        {
            return true;
        }

        return false;
    }

    #region ����¼�
    private void onClickLeft()
    {
        var m = maskList["left"];
        var t = m.GetComponentsInChildren<TMP_Text>(true);

        if (t[0].gameObject.activeSelf)
        {
            t[0].gameObject.SetActive(false);
            t[1].gameObject.SetActive(true);

            sm.CancelStateLog();
            sm.StateSwitchToSetting();
        }
        else if (t[1].gameObject.activeSelf)
        {
            t[1].gameObject.SetActive(false);
            t[2].gameObject.SetActive(true);

            sm.CancelStateSetting();
            sm.StateSwitchToNote();

        }
        else if (t[2].gameObject.activeSelf)
        {
            t[2].gameObject.SetActive(false);

            sm.CancelStateNote();
            sm.StateSwitchToLog();
            dioState.SetButtonsActive(false);

            m.gameObject.SetActive(false);

            //�л���rightMask
            maskList["right"].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("�ݣ���Bug�ˡ�");
        }
    }

    private void onClickRight()
    {
        maskList["right"].gameObject.SetActive(false);
        maskList["rightPanel"].gameObject.SetActive(true);
    }

    private void onClickRightPanel()
    {
        maskList["rightPanel"].gameObject.SetActive(false);
        dioState.SetButtonsActive(true);
    }

    private void onClickYellow()
    {
        maskList["yellow"].gameObject.SetActive(false);
        dioState.SetButtonsActive(true);
    }
    #endregion
}

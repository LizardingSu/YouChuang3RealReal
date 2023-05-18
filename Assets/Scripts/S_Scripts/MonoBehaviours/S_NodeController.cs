using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_NodeController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Sprite DefaultNode;
    public Sprite ActiveNode;

    private void OnEnable()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = DefaultNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DefaultNode.rect.width, DefaultNode.rect.height);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().sprite = ActiveNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(ActiveNode.rect.width, ActiveNode.rect.height);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        S_CalendarPanelManager calendarPanel = transform.parent.parent.parent.parent.GetComponent<S_CalendarPanelManager>();

        int day = calendarPanel.currentActiveDayButton.Value + 1;
        int id = calendarPanel.allDays[day - 1].Nodes[calendarPanel.currentNodes.IndexOf(this.gameObject)].ID;
        id = id - 1000 * day;

        Debug.Log("day" + day);
        Debug.Log("ID" + id);

        var sm = GameObject.Find("MainManager").GetComponent<S_StateManager>();
        sm.CancelStateCalendar();
        sm.StateSwitchToLog();

        //calendarPanel.accessor._DioLogueState.ReadToCurrentID(day, id);
        calendarPanel.accessor._SwitchSceneAnim.SwitchToNewScene(calendarPanel.accessor._DioLogueState.curData.date, (uint)day, id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().sprite = DefaultNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DefaultNode.rect.width, DefaultNode.rect.height);
    }
}

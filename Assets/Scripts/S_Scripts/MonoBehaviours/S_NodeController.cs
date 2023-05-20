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

        //Debug.Log("day" + day);
        //Debug.Log("ID" + id);

        calendarPanel.Accessor.TransAnim.SetActive(true);
        //calendarPanel.Accessor.TransAnim.GetComponent<Animator>().SetBool("PlayNow", true);

        if (day < calendarPanel.Accessor._DioLogueState.date)
        {
            calendarPanel.Accessor.TransAnim.GetComponent<Animator>().Play("TransAnimRE", 0);
        }
        else if (day == calendarPanel.Accessor._DioLogueState.date)
        {
            if (calendarPanel.Accessor._DioLogueState.ReadedList[id] == 1)
            {
                calendarPanel.Accessor.TransAnim.GetComponent<Animator>().Play("TransAnimRE", 0);
            }
            else
            {
                calendarPanel.Accessor.TransAnim.GetComponent<Animator>().Play("TransAnim", 0);
            }
        }
        else
        {
            calendarPanel.Accessor.TransAnim.GetComponent<Animator>().Play("TransAnim", 0);
        }

        StartCoroutine(JumpNode(day, id, calendarPanel.Accessor));
    }

    IEnumerator JumpNode(int day, int id, S_CentralAccessor accessor)
    {
        yield return new WaitForSeconds(0.8f);
        var sm = GameObject.Find("MainManager").GetComponent<S_StateManager>();
        sm.CancelStateCalendar();
        sm.StateSwitchToLog();

        accessor._DioLogueState.ReadToCurrentID(day, id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().sprite = DefaultNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DefaultNode.rect.width, DefaultNode.rect.height);
    }
}

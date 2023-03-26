using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class S_CalendarPanelManager : MonoBehaviour
{
    //存放16天对应按钮的数组
    public GameObject[] DayButtons;

    //slider伸长的长度
    public float SliderShowingHeight;

    //Node节点预制体
    public GameObject NodePrefab;

    //天数按钮对应slider的默认长度
    private float sliderDefaultHeight;

    //当前解锁天数
    private int dayNumber = 0;

    //当前正在进行的slider协程
    private List<Coroutine> currentCoroutines= new List<Coroutine>();

    //当前已激活的天数按钮
    private int? currentActiveDayButton = null;

    //当前所有实例化的节点预制体
    private List<GameObject> currentNodes = new List<GameObject>();

    //存放整个游戏流程16天节点数据的列表
    private List<S_DayInCalendar> allDays = new List<S_DayInCalendar>();

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        sliderDefaultHeight = ((RectTransform)(DayButtons[0].GetComponent<RectTransform>().GetChild(0))).rect.height;

        //test
        InitDayButtons(9);
        InitAllDays();

        //
        for (int i = 0; i < 5; i++)
        {
            currentNodes.Add(GameObject.Instantiate(NodePrefab, Vector3.zero, Quaternion.identity));
        }
    }

    /// <summary>
    /// 获取对话数据创建相应的S_NodeInDay和S_DayInCalendar对象实例并以此初始化allDays列表
    /// </summary>
    public void InitAllDays()
    {
        //测试代码
        for (int i = 0; i < 16; i++)
        {
            S_DayInCalendar day = new S_DayInCalendar(new List<S_NodeInDay>(), 400);
            S_NodeInDay node1 = new S_NodeInDay(day, "node1", "点击跳转1", (i + 1) * 10);
            S_NodeInDay node2 = new S_NodeInDay(day, "node2", "点击跳转2", (i + 1) * 20);
            day.Nodes.Add(node1);
            day.Nodes.Add(node2);
            allDays.Add(day);
        }
    }

    /// <summary>
    /// 使前day天的日期按钮可见且可操作
    /// </summary>
    /// <param name="day"></param>
    public void InitDayButtons(int day)
    {
        dayNumber = day;

        GameObject button;
        GameObject slider;
        GameObject mask;

        for (int i = 0; i < day; i++)
        {
            slider = DayButtons[i].GetComponent<RectTransform>().GetChild(0).gameObject;
            button = DayButtons[i].GetComponent<RectTransform>().GetChild(1).gameObject;
            mask = DayButtons[i].GetComponent<RectTransform>().GetChild(2).gameObject;

            slider.SetActive(true);
            button.SetActive(true);
            mask.SetActive(false);

            button.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();

            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(sliderRect.rect.width, sliderDefaultHeight);
        }
    }

    /// <summary>
    /// dayButton按钮的相应函数，需传入button序数(buttonIndex)以确定调用者
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void DayButtonClickFunc(int buttonIndex)
    {
        //停止当前日历面板正在进行的所有协程动画
        if (currentCoroutines.Count != 0)
        {
            foreach (Coroutine c in currentCoroutines)
            {
                StopCoroutine(c);
            }
            currentCoroutines.Clear();
        }

        //点击的是当前已激活项时
        if (currentActiveDayButton != null && currentActiveDayButton.Value == buttonIndex)
        {
            HideNodesInDay();
            RectTransform sliderRect = (RectTransform)DayButtons[buttonIndex].transform.GetChild(0);
            currentCoroutines.Add(StartCoroutine(SliderChangeTo(sliderDefaultHeight, sliderRect)));
            //currentActiveDayButton = null;
        }
        else
        {
            currentActiveDayButton = buttonIndex;
            HideNodesInDay();
            InitDayButtons(dayNumber);

            RectTransform sliderRect = (RectTransform)DayButtons[buttonIndex].transform.GetChild(0);
            currentCoroutines.Add(StartCoroutine(SliderChangeTo(SliderShowingHeight, sliderRect)));
        }
    }

    /// <summary>
    /// 显示当前激活按钮对应的当天的所有结点，在协程动画结束后调用
    /// </summary>
    public void ShowNodesInDay()
    {
        HideNodesInDay();
        GameObject slider = DayButtons[currentActiveDayButton.Value].GetComponent<RectTransform>().GetChild(0).gameObject;
        int currentNodeIndex = 0;
        foreach (S_NodeInDay node in allDays[currentActiveDayButton.Value].Nodes)
        {
            GameObject showedNode = currentNodes[currentNodeIndex];

            showedNode.SetActive(true);
            showedNode.transform.SetParent(slider.transform);

            float sliderHeight = slider.GetComponent<RectTransform>().rect.height;
            float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

            float posX = slider.GetComponent<RectTransform>().rect.width / 2 - showedNode.transform.GetChild(0).GetComponent<RectTransform>().rect.width / 2;
            float posY = sliderHeight / 2 - sliderWidth - (sliderHeight - sliderWidth) * node.CalculateLocationInFloat();

            showedNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            
            GameObject dialogBox = showedNode.transform.GetChild(1).gameObject;
            GameObject text = dialogBox.transform.GetChild(0).gameObject;

            text.GetComponent<Text>().text = node.Description;
            dialogBox.SetActive(false);

            currentNodes.Add(showedNode);

            currentNodeIndex++;
        }
    }

    /// <summary>
    /// 销毁当前所有实例化的节点预制体
    /// </summary>
    public void HideNodesInDay()
    {
        foreach (var showedNode in currentNodes)
        {
            showedNode.SetActive(false);
        }
    }

    /// <summary>
    /// slider缩放动画协程函数
    /// </summary>
    /// <param name="height"></param>
    /// <param name="RT"></param>
    /// <returns></returns>
    IEnumerator SliderChangeTo(float height, RectTransform RT)
    {
        float originHeight = RT.rect.height;
        int fixedFrameNumber = 20;

        bool needResetActiveButton = height < originHeight;

        for (int i = 0; i < fixedFrameNumber; i++)
        {
            yield return new WaitForFixedUpdate();
            //RT.rect.Set(RT.rect.x, RT.rect.y, RT.rect.width, Mathf.Lerp(originHeight, height, (float)i / 50f));
            if (i == fixedFrameNumber / 2)
            {
                RT.sizeDelta = new Vector2(RT.rect.width, height);
                ShowNodesInDay();
            }

            RT.sizeDelta = new Vector2(RT.rect.width, Mathf.Lerp(originHeight, height, (float)(i * i) / (float)fixedFrameNumber));
        }

        if (needResetActiveButton)
        {
            currentActiveDayButton = null;
        }

        RT.sizeDelta = new Vector2(RT.rect.width, height);
        currentCoroutines.Clear();
    }
}

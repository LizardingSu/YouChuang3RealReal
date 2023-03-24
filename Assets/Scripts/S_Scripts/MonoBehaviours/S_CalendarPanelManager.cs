using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class S_CalendarPanelManager : MonoBehaviour
{
    //存放16天对应按钮的数组
    public GameObject[] DayButtons;

    //slider伸长的长度
    public float SliderShowingHeight;

    //天数按钮对应slider的默认长度
    private float sliderDefaultHeight;

    //当前解锁天数
    private int dayNumber = 0;

    //当前正在进行的slider协程
    private List<Coroutine> currentCoroutines= new List<Coroutine>();

    //当前已激活的天数按钮
    private int? currentActiveDayButton = null;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        sliderDefaultHeight = ((RectTransform)(DayButtons[0].GetComponent<RectTransform>().GetChild(0))).rect.height;

        //test
        InitDayButtons(9);
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
        if (currentCoroutines.Count != 0)
        {
            foreach (Coroutine c in currentCoroutines)
            {
                StopCoroutine(c);
            }
            currentCoroutines.Clear();
        }

        if (currentActiveDayButton != null && currentActiveDayButton.Value == buttonIndex)
        {
            RectTransform sliderRect = (RectTransform)DayButtons[buttonIndex].transform.GetChild(0);
            currentCoroutines.Add(StartCoroutine(SliderChangeTo(sliderDefaultHeight, sliderRect)));
            currentActiveDayButton = null;
        }
        else
        {
            currentActiveDayButton = buttonIndex;
            InitDayButtons(dayNumber);

            RectTransform sliderRect = (RectTransform)DayButtons[buttonIndex].transform.GetChild(0);
            currentCoroutines.Add(StartCoroutine(SliderChangeTo(SliderShowingHeight, sliderRect)));
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
        for (int i = 0; i < fixedFrameNumber; i++)
        {
            //RT.rect.Set(RT.rect.x, RT.rect.y, RT.rect.width, Mathf.Lerp(originHeight, height, (float)i / 50f));
            RT.sizeDelta = new Vector2(RT.rect.width, Mathf.Lerp(originHeight, height, (float)(i * i) / (float)fixedFrameNumber));
            yield return new WaitForFixedUpdate();
        }

        RT.sizeDelta = new Vector2(RT.rect.width, height);
        currentCoroutines.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class S_CalendarPanelManager : MonoBehaviour
{
    //���16���Ӧ��ť������
    public GameObject[] DayButtons;

    //slider�쳤�ĳ���
    public float SliderShowingHeight;

    //������ť��Ӧslider��Ĭ�ϳ���
    private float sliderDefaultHeight;

    //��ǰ��������
    private int dayNumber = 0;

    //��ǰ���ڽ��е�sliderЭ��
    private List<Coroutine> currentCoroutines= new List<Coroutine>();

    //��ǰ�Ѽ����������ť
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
    /// ʹǰday������ڰ�ť�ɼ��ҿɲ���
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
    /// dayButton��ť����Ӧ�������贫��button����(buttonIndex)��ȷ��������
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
    /// slider���Ŷ���Э�̺���
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

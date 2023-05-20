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

    //accessor
    public S_CentralAccessor Accessor;

    //slider伸长的长度
    public float SliderShowingHeight;

    //Node节点预制体
    public GameObject NodePrefab;

    //天数按钮对应slider的默认长度
    private float sliderDefaultHeight;

    //当前解锁天数
    public int dayNumber = 0;

    //当前正在进行的slider协程
    private List<Coroutine> currentCoroutines= new List<Coroutine>();

    //当前已激活的天数按钮
    public int? currentActiveDayButton = null;

    //当前所有实例化的节点预制体
    public List<GameObject> currentNodes = new List<GameObject>();

    //存放整个游戏流程16天节点数据的列表
    public List<S_DayInCalendar> allDays = new List<S_DayInCalendar>();

    /// <summary>
    /// OnEnable
    /// </summary>
    private void OnEnable()
    {
        sliderDefaultHeight = ((RectTransform)(DayButtons[0].GetComponent<RectTransform>().GetChild(0))).rect.height;

        //dayNumber = 0;

        //Debug.Log(dayNumber);
        InitAllDays();
        InitDayButtons();

        //
        if(currentNodes.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                currentNodes.Add(GameObject.Instantiate(NodePrefab, Vector3.zero, Quaternion.identity));
            }
        }
    }



    /// <summary>
    /// 获取对话数据创建相应的S_NodeInDay和S_DayInCalendar对象实例并以此初始化allDays列表
    /// </summary>
    public void InitAllDays()
    {
        dayNumber = 0;
        foreach (var i in Accessor.ProcessManager.m_Saving.Choices)
        {
            int id = i.ID;
            if (dayNumber < (id / 1000))
            {
                dayNumber = id / 1000;
            }
        }

        allDays.Clear();
        for (int i = 0; i < dayNumber; i++)
        {
            
            S_DayInCalendar day = new S_DayInCalendar(new List<S_NodeInDay>(), 100);
            foreach (var cho in Accessor.ProcessManager.m_Saving.Choices)
            {
                if ((cho.ID / 1000) == i + 1)
                {
                    bool setGrey = false;
                    setGrey = !(cho.Answer == string.Empty);
                    if (cho.ID % 1000 == 0 || cho.Choice == -2)
                    {
                        setGrey = true;
                    }

                    day.Nodes.Add(new S_NodeInDay(day, cho.ID, "点击跳转", 0, setGrey));
                }

                
            }

            for (int k = day.Nodes.Count - 1; k > 0; k--)
            {
                for (int j = 0; j < k; j++)
                {
                    if (day.Nodes[j].ID > day.Nodes[j + 1].ID)
                    {
                        S_NodeInDay t = day.Nodes[j + 1];
                        day.Nodes[j + 1] = day.Nodes[j];
                        day.Nodes[j] = t;
                    }
                }
            }
            
            if (day.Nodes.Count == 1)
            {
                day.Nodes[0].Location = 0;
            }
            else
            {
                for (int j = 0; j < day.Nodes.Count; j++)
                {
                    day.Nodes[j].Location = (int)((float)j / (float)(day.Nodes.Count - 1) * 100f);
                }
            }
            

            allDays.Add(day);
        }
    }

    /// <summary>
    /// 使前dayNumber天的日期按钮可见且可操作
    /// </summary>
    public void InitDayButtons()
    {
        //dayNumber = day;

        GameObject button;
        GameObject slider;
        GameObject mask;

        for (int i = 0; i < 16; i++)
        {
            slider = DayButtons[i].GetComponent<RectTransform>().GetChild(0).gameObject;
            button = DayButtons[i].GetComponent<RectTransform>().GetChild(1).gameObject;
            mask = DayButtons[i].GetComponent<RectTransform>().GetChild(2).gameObject;

            slider.SetActive(false);
            button.SetActive(false);
            mask.SetActive(true);
        }

        for (int i = 0; i < dayNumber; i++)
        {
            slider = DayButtons[i].GetComponent<RectTransform>().GetChild(0).gameObject;
            button = DayButtons[i].GetComponent<RectTransform>().GetChild(1).gameObject;
            mask = DayButtons[i].GetComponent<RectTransform>().GetChild(2).gameObject;

            slider.SetActive(true);
            button.SetActive(true);
            mask.SetActive(false);

            button.GetComponent<UnityEngine.UI.Image>().material = null;

            button.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();

            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(sliderRect.rect.width, sliderDefaultHeight);
        }

        DayButtons[Accessor._DioLogueState.date - 1].GetComponent<RectTransform>().GetChild(1).GetComponent<UnityEngine.UI.Image>().material = Accessor.StateManager.NotePanel.GetComponent<S_NotePanelManager>().OutlineMaterial;
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
            foreach (var button in DayButtons)
            {
                button.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
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
            InitDayButtons();

            RectTransform sliderRect = (RectTransform)DayButtons[buttonIndex].transform.GetChild(0);
            Debug.Log("slider" + sliderDefaultHeight);
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
        GameObject line = slider.transform.GetChild(0).gameObject;
        int currentNodeIndex = 0;
        Debug.Log(currentActiveDayButton);
        Debug.Log(allDays[currentActiveDayButton.Value].Nodes.Count);
        foreach (S_NodeInDay node in allDays[currentActiveDayButton.Value].Nodes)
        {

            GameObject showedNode = currentNodes[currentNodeIndex];

            showedNode.SetActive(true);
            showedNode.transform.SetParent(line.transform);

            float lineHeight = line.GetComponent<RectTransform>().rect.height;

            float posX = 0;
            float posY = lineHeight / 2 - lineHeight * node.CalculateLocationInFloat();

            showedNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

            if (node.IsCorrect)
            {
                showedNode.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 0.5f); 
            }
            else
            {
                showedNode.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f);
            }

            GameObject dialogBox = showedNode.transform.GetChild(1).gameObject;
            GameObject text = dialogBox.transform.GetChild(0).gameObject;

            text.GetComponent<Text>().text = node.Description;
            dialogBox.SetActive(false);

            Debug.Log(currentNodes.Count);

            //currentNodes.Add(showedNode);

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
        RT.parent.GetChild(1).GetComponent<UnityEngine.UI.Button>().interactable = false;
        float originHeight = RT.rect.height;
        int fixedFrameNumber = 20;

        bool shortening = height < originHeight;

        for (int i = 0; i < fixedFrameNumber; i++)
        {
            yield return new WaitForFixedUpdate();
            //RT.rect.Set(RT.rect.x, RT.rect.y, RT.rect.width, Mathf.Lerp(originHeight, height, (float)i / 50f));
            if (i == fixedFrameNumber / 2)
            {
                RT.sizeDelta = new Vector2(RT.rect.width, height);
                if (!shortening)
                ShowNodesInDay();
            }

            RT.sizeDelta = new Vector2(RT.rect.width, Mathf.Lerp(originHeight, height, (float)(i * i) / (float)fixedFrameNumber));
        }

        if (shortening)
        {
            currentActiveDayButton = null;
        }

        RT.sizeDelta = new Vector2(RT.rect.width, height);
        RT.parent.GetChild(1).GetComponent<UnityEngine.UI.Button>().interactable = true;
        currentCoroutines.Clear();
    }
}

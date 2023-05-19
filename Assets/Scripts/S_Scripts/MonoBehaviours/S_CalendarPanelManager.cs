using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class S_CalendarPanelManager : MonoBehaviour
{
    //���16���Ӧ��ť������
    public GameObject[] DayButtons;

    //accessor
    public S_CentralAccessor Accessor;

    //slider�쳤�ĳ���
    public float SliderShowingHeight;

    //Node�ڵ�Ԥ����
    public GameObject NodePrefab;

    //������ť��Ӧslider��Ĭ�ϳ���
    private float sliderDefaultHeight;

    //��ǰ��������
    public int dayNumber = 0;

    //��ǰ���ڽ��е�sliderЭ��
    private List<Coroutine> currentCoroutines= new List<Coroutine>();

    //��ǰ�Ѽ����������ť
    public int? currentActiveDayButton = null;

    //��ǰ����ʵ�����Ľڵ�Ԥ����
    public List<GameObject> currentNodes = new List<GameObject>();

    //���������Ϸ����16��ڵ����ݵ��б�
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
    /// ��ȡ�Ի����ݴ�����Ӧ��S_NodeInDay��S_DayInCalendar����ʵ�����Դ˳�ʼ��allDays�б�
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

                    day.Nodes.Add(new S_NodeInDay(day, cho.ID, "�����ת", 0, setGrey));
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
    /// ʹǰdayNumber������ڰ�ť�ɼ��ҿɲ���
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
    /// dayButton��ť����Ӧ�������贫��button����(buttonIndex)��ȷ��������
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void DayButtonClickFunc(int buttonIndex)
    {
        //ֹͣ��ǰ����������ڽ��е�����Э�̶���
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

        //������ǵ�ǰ�Ѽ�����ʱ
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
    /// ��ʾ��ǰ���ť��Ӧ�ĵ�������н�㣬��Э�̶������������
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
    /// ���ٵ�ǰ����ʵ�����Ľڵ�Ԥ����
    /// </summary>
    public void HideNodesInDay()
    {
        foreach (var showedNode in currentNodes)
        {
            showedNode.SetActive(false);
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

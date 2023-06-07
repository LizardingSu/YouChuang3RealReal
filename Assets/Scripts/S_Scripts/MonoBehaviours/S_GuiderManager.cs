using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_GuiderManager : MonoBehaviour, IPointerDownHandler
{
    public S_CentralAccessor accessor;

    public List<bool> GuiderList = new List<bool>();

    public int GuiderCount = 5;

    [Header("�̳�ͼ��")]
    public Sprite[] Guider1;
    public Sprite[] Guider2;
    public Sprite[] Guider3;
    public Sprite[] Guider4;
    public Sprite[] Guider5;

    private Sprite[] curSprites;

    private int curPage;

    /// <summary>
    /// ��⵱ǰ�Ƿ���Ҫ�������ֽ̳̣������Ҫ�Ļ�ֱ�ӽ������ֽ̳̣�������̳���᷵��true
    /// </summary>
    /// <returns></returns>
    public bool GuideCheck(bool gameGuider = false)
    {
        if (accessor._DioLogueState.curData == null)
            return false;
        int date = (int)accessor._DioLogueState.curData.date;
        int idx = (int)accessor._DioLogueState.curData.nextIdx;

        Debug.Log(GuiderList.Count);

        if (date == 1 && idx == 7 && GuiderList[0])      //�̳�1
        {
            StartGuider(Guider1);
            GuiderList[0] = false;
            accessor.ProcessManager.SaveGuider();
            return true;
        }
        if (date == 1 && idx == 26 && GuiderList[1])      //�̳�2
        {
            StartGuider(Guider2);
            GuiderList[1] = false;
            accessor.ProcessManager.SaveGuider();
            return true;
        }
        if (date == 1 && idx == 39 && gameGuider && GuiderList[2])      //�̳�3
        {
            StartGuider(Guider3);
            GuiderList[2] = false;
            accessor.ProcessManager.SaveGuider();
            return true;
        }
        if (date == 1 && idx == 270 && GuiderList[3])      //�̳�4
        {
            StartGuider(Guider4);
            GuiderList[3] = false; 
            accessor.ProcessManager.SaveGuider();
            return true;
        }
        if (date == 2 && idx == 1 && GuiderList[4])      //�̳�2
        {
            StartGuider(Guider5);
            GuiderList[4] = false;
            accessor.ProcessManager.SaveGuider();
            return true;
        }

        return false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (curPage == curSprites.Length - 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetComponent<Image>().sprite = curSprites[++curPage];
        }
    }

    public void StartGuider(Sprite[] guiders)
    {
        this.gameObject.SetActive(true);
        curSprites = guiders;
        GetComponent<Image>().sprite = curSprites[0];
        curPage = 0;
    }
}

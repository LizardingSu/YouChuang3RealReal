using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class S_MenuManager : MonoBehaviour
{
    public S_CentralAccessor accessor;

    [Range(0.1f, 10f)]
    public float BreatheSpeed;

    [Range(0f, 100f)]
    public float BreatheRange;

    private Transform ButtonsTransform;

    private Transform HeadLineTransform;

    private bool BreatheStart;

    public void ShowMenu(bool show)
    {
        if (show)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Temp_HideMenu()
    {
        ShowMenu(false);
    }

    public void NewGame()
    {
        accessor._DioLogueState.LoadNewSceneImmediate();
        Invoke("Temp_HideMenu", 1.2f);
    }

    public void ContinueGame()
    {
        accessor._DioLogueState.DelayedContinueGame();
        //Invoke("Temp_HideMenu", 1f);
    }

    private void Start()
    {
        BreatheStart = false;
        ButtonsTransform = transform.Find("Buttons");
        HeadLineTransform = transform.Find("HeadLine");
        for (int i = 0; i < ButtonsTransform.childCount; i++)
        {
            ButtonsTransform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
        }
        if (!File.Exists(Application.persistentDataPath + "/ApodaSaving/" + accessor.ProcessManager.m_SavingName1))
        {
            ButtonsTransform.GetChild(1).GetComponent<Button>().interactable = false;
        }

        Sequence s = DOTween.Sequence();
        if (true)   //之后改成大一轮和大二轮的分支
        {
            s.Insert(0.5f, HeadLineTransform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, HeadLineTransform.transform.position.z), 0.5f));
            s.Insert(0.5f, HeadLineTransform.GetComponent<Image>().DOColor(new Color(HeadLineTransform.GetComponent<Image>().color.r, HeadLineTransform.GetComponent<Image>().color.g, HeadLineTransform.GetComponent<Image>().color.b, 1f), 0.5f));

            for (int i = 0; i < ButtonsTransform.childCount; i++)
            {
                s.Insert(1f + 0.2f * i, ButtonsTransform.GetChild(i).DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, ButtonsTransform.GetChild(i).transform.position.z), 0.5f));
                s.Insert(1f + 0.2f * i, ButtonsTransform.GetChild(i).GetComponent<Image>().DOColor(new Color(ButtonsTransform.GetChild(i).GetComponent<Image>().color.r, ButtonsTransform.GetChild(i).GetComponent<Image>().color.g, ButtonsTransform.GetChild(i).GetComponent<Image>().color.b, 1f), 0.5f));
            }
            s.AppendCallback(() => { BreatheStart = true; Debug.Log("BreatheStart"); });
        }
        else
        {

        }
    }

    private void Update()
    {
        if (BreatheStart)
        {
            HeadLineTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Sin(Time.time / BreatheSpeed) * BreatheRange);
        }
    }
}

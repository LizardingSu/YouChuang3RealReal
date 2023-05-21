using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class S_MenuManager : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public GameObject MenuSettingPanel;

    [Range(0.1f, 10f)]
    public float BreatheSpeed;

    [Range(0f, 100f)]
    public float BreatheRange;

    private Transform ButtonsTransform;

    private Transform HeadLineTransform;

    private bool BreatheStart;

    private float MenuSettingPaenlStartX;

    private RectTransform bgmMaskRT;

    private RectTransform seMaskRT;

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

    public void ShowSettingPanel()
    {
        MenuSettingPanel.SetActive(true);

        //accessor.ProcessManager.LoadProfile();
        //float bgmValue = accessor.ProcessManager.m_Profile.BGMVolume;
        //float seValue = accessor.ProcessManager.m_Profile.SEVolume;

        //float bgmOriginWidth = bgmMaskRT.GetChild(0).GetComponent<S_SliderFillFunction>().maskOriginWidth;
        //float seOriginWidth = seMaskRT.GetChild(0).GetComponent<S_SliderFillFunction>().maskOriginWidth;

        //Debug.Log(bgmOriginWidth);
        //Debug.Log(seOriginWidth);

        //bgmMaskRT.sizeDelta = new Vector2(bgmValue * bgmOriginWidth, bgmMaskRT.sizeDelta.y);
        //seMaskRT.sizeDelta = new Vector2(seValue * seOriginWidth, seMaskRT.sizeDelta.y);

        MenuSettingPanel.transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, MenuSettingPanel.transform.position.z), 0.5f);
        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f), 0.5f);
        }
    }

    public void HideSettingPanel()
    {
        Debug.Log("Hide");
        Sequence s = DOTween.Sequence();
        s.Append(MenuSettingPanel.transform.DOMove(new Vector3(MenuSettingPaenlStartX, Screen.height * 0.5f, MenuSettingPanel.transform.position.z), 0.5f));
        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            s.Insert(0, images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f), 0.5f));
        }
        s.AppendCallback(() => { MenuSettingPanel.SetActive(false); });
    }

    private void Start()
    {
        BreatheStart = false;
        ButtonsTransform = transform.Find("Buttons");
        HeadLineTransform = transform.Find("HeadLine");

        MenuSettingPanel.SetActive(false);

        bgmMaskRT = transform.Find("SettingPanel/BGMSlider/FillMask").GetComponent<RectTransform>();
        seMaskRT = transform.Find("SettingPanel/SESlider/FillMask").GetComponent<RectTransform>();

        GameObject SettingCross = transform.Find("SettingPanel/Cross").gameObject;
        SettingCross.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;

        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
        }

        MenuSettingPaenlStartX = MenuSettingPanel.transform.position.x;

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

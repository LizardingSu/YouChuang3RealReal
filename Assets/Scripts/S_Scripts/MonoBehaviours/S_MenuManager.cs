using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using DG.Tweening.Plugins.Core.PathCore;

public class S_MenuManager : MonoBehaviour
{
    public AudioClip MenuBGM;

    public S_CentralAccessor accessor;

    public GameObject MenuSettingPanel;
    public GameObject MenuNotePanel;

    public Sprite NewGameNote;

    public Sprite ExitGameNote;

    public GameObject BlackSwitcher;

    public float BlackSwitcherUpDelta;

    [Range(0.1f, 10f)]
    public float BreatheSpeed;

    [Range(0f, 100f)]
    public float BreatheRange;

    public Transform ButtonsTransform;

    private Transform HeadLineTransform;

    private bool BreatheStart;

    private float MenuSettingAndNotePaenlStartX;

    private float BlackSwitcherOriginY;

    private RectTransform bgmMaskRT;

    private RectTransform seMaskRT;

    private bool NewGameNoteShowing;

    private bool ExitGameNoteShowing;

    private DG.Tweening.Sequence NotePanelSequence;
    private DG.Tweening.Sequence SettingPanelSequence;


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

    public void SwitchToMenu()
    {
        BlackSwitcher.SetActive(true);
        var s = DOTween.Sequence();

        BlackSwitcher.transform.position = new Vector3(BlackSwitcher.transform.position.x, BlackSwitcherOriginY, BlackSwitcher.transform.position.z);
        BlackSwitcher.GetComponent<Image>().color = Color.white;
        BlackSwitcher.GetComponent<Image>().raycastTarget = true;


        s.Append(BlackSwitcher.transform.DOMoveY(BlackSwitcherOriginY + BlackSwitcherUpDelta, 1.2f));
        s.AppendCallback(() =>
        {
            HideSettingPanel();
            OnClickNo();
            accessor.AudioManager.PlayBGM(MenuBGM);
            accessor.StateManager.CancelCurrentState();
            accessor.StateManager.StateSwitchToLog();
            gameObject.SetActive(true);
        });
        s.AppendInterval(0.3f);
        s.Append(BlackSwitcher.GetComponent<Image>().DOColor(new Color(Color.white.r, Color.white.g, Color.white.b, 0f), 1f));
        s.AppendCallback( () => 
        {
            BlackSwitcher.GetComponent<Image>().raycastTarget = false;
            BlackSwitcher.SetActive(false);
        });

        if (!File.Exists(Application.persistentDataPath + "/ApodaSaving/" + accessor.ProcessManager.m_SavingName1))
        {
            ButtonsTransform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else
        {
            ButtonsTransform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }

    public void Temp_HideMenu()
    {
        ShowMenu(false);
    }

    public void NewGame(int i)
    {
        //accessor._DioLogueState.LoadNewSceneImmediate();
        //Invoke("Temp_HideMenu", 1.2f);
        accessor._DioLogueState.LoadNewSceneImmediate(i);

        BlackSwitcher.SetActive(true);
        var s = DOTween.Sequence();

        BlackSwitcher.transform.position = new Vector3(BlackSwitcher.transform.position.x, BlackSwitcherOriginY, BlackSwitcher.transform.position.z);
        BlackSwitcher.GetComponent<Image>().color = Color.white;
        BlackSwitcher.GetComponent<Image>().raycastTarget = true;


        s.Append(BlackSwitcher.transform.DOMoveY(BlackSwitcherOriginY + BlackSwitcherUpDelta, 1.2f));
        s.AppendCallback(() =>
        {
            HideSettingPanel();
            OnClickNo();
            //accessor.AudioManager.PlayBGM(MenuBGM);
            //accessor.StateManager.CancelCurrentState();
            //accessor.StateManager.StateSwitchToLog();
            gameObject.SetActive(false);
            accessor._DioLogueState.ReadToCurrentID(i, -1);
        });
        s.AppendInterval(0.3f);
        s.Append(BlackSwitcher.GetComponent<Image>().DOColor(new Color(Color.white.r, Color.white.g, Color.white.b, 0f), 1f));
        s.AppendCallback(() =>
        {
            accessor._DioLogueState.ProcessInput();
        });
        s.AppendInterval(1f);
        s.AppendCallback(() =>
        {
            BlackSwitcher.GetComponent<Image>().raycastTarget = false;
            BlackSwitcher.SetActive(false);
            accessor._DioLogueState.SetButtonsActive(true);
        });
        //s.AppendCallback(() =>
        //{

        //});
        accessor.ProcessManager.DeleteFile(accessor.ProcessManager.m_GuiderName);
        accessor.ProcessManager.LoadGuider();
    }


    public void ContinueGame()
    {
        accessor.ProcessManager.LoadGuider();
        accessor._DioLogueState.DelayedContinueGame();
        //Invoke("Temp_HideMenu", 1f);
    }

    public void ShowSettingPanel()
    {
        if (SettingPanelSequence != null)
        {
            SettingPanelSequence.Complete();
        }
        else
        {
            SettingPanelSequence = DOTween.Sequence();
        }

        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        //MenuSettingPanel.transform.DOKill(true);
        //for (int i = 0; i < images.Length; i++)
        //{
        //    if (images[i].gameObject.name != "ClickArea")
        //        images[i].DOKill(true);
        //}
        
        MenuSettingPanel.SetActive(true);

        OnClickNo();

        accessor.ProcessManager.LoadProfile();
        float bgmValue = accessor.ProcessManager.m_Profile.BGMVolume;
        float seValue = accessor.ProcessManager.m_Profile.SEVolume;

        float bgmOriginWidth = bgmMaskRT.parent.GetChild(0).GetComponent<S_NewSliderFunction>().MaskOriginWidth;
        float seOriginWidth = seMaskRT.parent.GetChild(0).GetComponent<S_NewSliderFunction>().MaskOriginWidth;

        //Debug.Log(bgmOriginWidth);
        //Debug.Log(seOriginWidth);

        bgmMaskRT.sizeDelta = new Vector2(bgmValue * bgmOriginWidth, bgmMaskRT.sizeDelta.y);
        seMaskRT.sizeDelta = new Vector2(seValue * seOriginWidth, seMaskRT.sizeDelta.y);


        SettingPanelSequence.Append(MenuSettingPanel.transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, MenuSettingPanel.transform.position.z), 0.5f));
        
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].gameObject.name != "ClickArea")
                SettingPanelSequence.Insert(0, images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f), 0.5f));
        }
    }

    public void HideSettingPanel()
    {
        if (SettingPanelSequence != null)
        {
            SettingPanelSequence.Complete();
        }
        else
        {
            SettingPanelSequence = DOTween.Sequence();
        }

        Debug.Log("Hide");
        SettingPanelSequence.Append(MenuSettingPanel.transform.DOMove(new Vector3(MenuSettingAndNotePaenlStartX, Screen.height * 0.5f, MenuSettingPanel.transform.position.z), 0.5f));
        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].gameObject.name != "ClickArea")
                SettingPanelSequence.Insert(0, images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f), 0.5f));
        }
        SettingPanelSequence.AppendCallback(() => { MenuSettingPanel.SetActive(false); });
    }

    public void ShowNotePanel(bool newGame)
    {
        if (NotePanelSequence!= null)
        {
            NotePanelSequence.Complete();
        }
        else
        {
            NotePanelSequence = DOTween.Sequence();
        }

        Image[] images = MenuNotePanel.transform.GetComponentsInChildren<Image>();
        //for (int i = 0; i < images.Length; i++)
        //{
        //    images[i].DOKill(true);
        //}

        MenuNotePanel.SetActive(true);
        HideSettingPanel();

        if (newGame)
        {
            MenuNotePanel.transform.GetChild(0).GetComponent<Image>().sprite = NewGameNote;
            NewGameNoteShowing = true;
        }
        else
        {
            MenuNotePanel.transform.GetChild(0).GetComponent<Image>().sprite = ExitGameNote;
            ExitGameNoteShowing = true;
        }

        NotePanelSequence.Append(MenuNotePanel.transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, MenuNotePanel.transform.position.z), 0.5f));
        for (int i = 0; i < images.Length; i++)
        {
            NotePanelSequence.Insert(0, images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1f), 0.5f));
        }
    }

    public void OnClickYes()
    {
        if (NewGameNoteShowing)
        {
            NewGame(0);
        }

        if (ExitGameNoteShowing)
        {
            accessor.ProcessManager.ExitGame();
        }
    }

    public void OnClickNo()
    {
        NewGameNoteShowing = false;
        ExitGameNoteShowing = false;

        if (NotePanelSequence != null)
        {
            NotePanelSequence.Complete();
        }
        else
        {
            NotePanelSequence = DOTween.Sequence();
        }

        NotePanelSequence.Append(MenuNotePanel.transform.DOMove(new Vector3(MenuSettingAndNotePaenlStartX, Screen.height * 0.5f, MenuNotePanel.transform.position.z), 0.5f));
        Image[] images = MenuNotePanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            NotePanelSequence.Insert(0, images[i].DOColor(new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f), 0.5f));
        }
        NotePanelSequence.AppendCallback(() => { MenuNotePanel.SetActive(false); });
        
    }

    private void Start()
    {
        BreatheStart = false;
        ButtonsTransform = transform.Find("Buttons");
        HeadLineTransform = transform.Find("HeadLine");

        BlackSwitcherOriginY = BlackSwitcher.transform.position.y;

        NewGameNoteShowing = false;
        ExitGameNoteShowing = false;

        MenuSettingPanel.SetActive(false);
        MenuNotePanel.SetActive(false);

        bgmMaskRT = transform.Find("SettingPanel/BGMSlider/FillMask").GetComponent<RectTransform>();
        seMaskRT = transform.Find("SettingPanel/SESlider/FillMask").GetComponent<RectTransform>();

        GameObject SettingCross = transform.Find("SettingPanel/Cross").gameObject;
        SettingCross.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;

        Image[] images = MenuSettingPanel.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
        }

        MenuSettingAndNotePaenlStartX = MenuSettingPanel.transform.position.x;

        for (int i = 0; i < ButtonsTransform.childCount; i++)
        {
            ButtonsTransform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
        }
        if (!File.Exists(Application.persistentDataPath + "/ApodaSaving/" + accessor.ProcessManager.m_SavingName1))
        {
            ButtonsTransform.GetChild(1).GetComponent<Button>().interactable = false;
        }

        MenuNotePanel.transform.GetChild(1).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
        MenuNotePanel.transform.GetChild(2).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;

        var s = DOTween.Sequence();
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

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    accessor._DioLogueState.LoadScene4();
        //    Invoke("Temp_HideMenu", 1.2f);
        //}
    }
}

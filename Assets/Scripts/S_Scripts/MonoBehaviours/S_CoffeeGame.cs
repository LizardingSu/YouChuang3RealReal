using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CoffeeGame : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public GameObject GamePanel;

    public int frameNumber = 30;

    public float GamePanelShowedPosY;

    public float GamePanelOriginPosY;

    private void Start()
    {
        DioLogueState ds = accessor._DioLogueState;
        ds.dialogueChanged.AddListener(StartCoffeeGame);
        GamePanelOriginPosY = GamePanel.GetComponent<RectTransform>().anchoredPosition.y;
    }

    public void StartCoffeeGame(DiologueData data)
    {
        if (data.processState == ProcessState.Coffee)
        {
            StartCoroutine(ShowGamePanel());
        }
        else
        {
            return;
        }
    }

    public void EndCoffeeGame()
    {
        StartCoroutine(HideGamePanel());
        accessor._DioLogueState.UpdateDiologue();
    }

    IEnumerator ShowGamePanel()
    {
        for (int i = 1; i <= frameNumber; i++)
        {
            float currentY = Mathf.Lerp(GamePanelOriginPosY, GamePanelShowedPosY, (float)i / (float)frameNumber);
            GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
            yield return 0;
        }
    }

    IEnumerator HideGamePanel()
    {
        for (int i = 1; i <= frameNumber; i++)
        {
            float currentY = Mathf.Lerp(GamePanelShowedPosY, GamePanelOriginPosY, (float)i / (float)frameNumber);
            GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
            yield return 0;
        }
    }

    private void Update()
    {
        //≤‚ ‘
        if (Input.GetKeyDown(KeyCode.G))
        {
            //var test = new DiologueData(0, ProcessState.Coffee, 0, 0, 0, 0, CharacterState.In, null, null);
            StartCoroutine(ShowGamePanel());
            //GameObject.Find("Main Camera").GetComponent<AudioSource>().Play();
        }
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            StartCoroutine(HideGamePanel());
        }
    }
}

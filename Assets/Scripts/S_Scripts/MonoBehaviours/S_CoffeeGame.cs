using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CoffeeGame : MonoBehaviour
{
    public S_CentralAccessor accessor;

    public GameObject GamePanel;

    public float GamePanelShowedPosY;

    private float GamePanelOriginPosY;

    private int frameNumber = 30;

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
            GamePanel.SetActive(true);
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
            float currentY = Mathf.Lerp(GamePanelOriginPosY, GamePanelShowedPosY, i / frameNumber);
            GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
            yield return null;
        }
    }

    IEnumerator HideGamePanel()
    {
        for (int i = 1; i <= frameNumber; i++)
        {
            float currentY = Mathf.Lerp(GamePanelShowedPosY, GamePanelOriginPosY, i / frameNumber);
            GamePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(GamePanel.GetComponent<RectTransform>().anchoredPosition.x, currentY);
            yield return null;
        }
        GamePanel.SetActive(false);
    }
}

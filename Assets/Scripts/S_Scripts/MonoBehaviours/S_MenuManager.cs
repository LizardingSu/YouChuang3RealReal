using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MenuManager : MonoBehaviour
{
    public S_CentralAccessor accessor;

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
}

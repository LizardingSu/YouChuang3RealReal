using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MenuManager : MonoBehaviour
{
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


}

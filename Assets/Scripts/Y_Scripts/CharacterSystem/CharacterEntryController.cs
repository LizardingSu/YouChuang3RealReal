using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEntryController : MonoBehaviour
{
    public Image image;
    public int CharID;
    public int ImageID;
    public void SetAllDatas(bool active,int charID=-1,int imageID=-1)
    {
        CharID = charID;
        ImageID = imageID;
        gameObject.SetActive(active);
    }
}

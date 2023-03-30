using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEntryController : MonoBehaviour
{
    public Image image;
    public int CharID;
    public string Name;
    public int ImageID;
    public void SetAllDatas(bool active,int charID = -1,string name = "",int imageID = -1)
    {
        Debug.Log(imageID);
        CharID = charID;
        Name = name;
        ImageID = imageID;
        gameObject.SetActive(active);

        if (imageID == -1 || active == false) return;

        Debug.Log(imageID);
        Debug.Log("Character/" + name + "/" + name + imageID);
        var tex = Resources.Load<Texture2D>("Character/" + name + "/" + name + imageID);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width,tex.height);
    }
}

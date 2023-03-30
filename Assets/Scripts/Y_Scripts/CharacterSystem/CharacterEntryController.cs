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
    public void SetAllDatas(bool active,int charID =-1,string name = "",int imageID=-1)
    {
        CharID = charID;
        Name = name;
        ImageID = imageID;
        gameObject.SetActive(active);

        if (imageID == -1 || active == false) return;

        var tex = Resources.Load<Texture2D>("Characters/" + name + "/" + name + imageID + ".png");
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }
}

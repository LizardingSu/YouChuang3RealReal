using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CurState
{
    UP,
    HIDE,
    DOWN
}

public class CharacterEntryController : MonoBehaviour
{
    public CurState curState = CurState.DOWN;
    public RectTransform rectTransform;
    public Image image;
    public TMP_Text nameBox;

    public int CharID;
    public string Name;
    public int ImageID;
    public void SetAllDatas(bool active,int charID = -1,string name = "",int imageID = -1)
    {
        CharID = charID;
        Name = name;
        ImageID = imageID;
        gameObject.SetActive(active);

        if (active == false) return;

        if(imageID == -1)
        {
            //set alpha to zero
            image.color = new Color(1,1,1,0);
            return;
        }
        else
        {
            image.color = new Color(1, 1, 1, 1);
        }

        var tex = Resources.Load<Texture2D>("Character/" + name + "/" + name + imageID);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width,tex.height);
    }

    /// <summary>
    /// 收到最下方为 -1000
    /// 收到看不到名字为 -80
    /// 正常为0
    /// </summary>
    public void MoveHideName(float time)
    {
        if (curState == CurState.HIDE) return;
        
        
        curState = CurState.HIDE;

        //not really need
        rectTransform.DOKill(true);

        rectTransform.DOAnchorPosY(-80, time);
    }
    public void MoveUp(float time)
    {
        if (curState == CurState.UP) return;

        curState = CurState.UP;

        //not really need
        rectTransform.DOKill(true);

        rectTransform.DOAnchorPosY(0, time);
    }
    public void MoveDown(float time)
    {
        if (curState == CurState.DOWN) return;

        curState = CurState.DOWN;

        //not really need
        rectTransform.DOKill(true);

        rectTransform.DOAnchorPosY(-1000, time);
    }

    public void FadeAnim(float time)
    {
        var rc = image.GetComponent<RectTransform>();
        rc.anchoredPosition = new Vector2(264, 0);

        //not really need
        rc.DOKill(true);

        rc.DOAnchorPosX(0, time);
    }

    public void KillAllAnim()
    {
        image.GetComponent<RectTransform>().DOKill(true);
        rectTransform.DOKill(true);
    }

    public void SetName(string name)
    {
        if(name!= "")
        {
            nameBox.text = name;
            nameBox.fontSize = 42-(name.Count()-1)*2;
        }
    }

    public void LateUpdate()
    {

    }
}

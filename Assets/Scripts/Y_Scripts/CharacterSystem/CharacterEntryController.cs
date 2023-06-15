using DG.Tweening;
using DG.Tweening.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    public Image whiteMask;

    public TMP_Text nameBox;

    Coroutine c = null;
    Sequence s;

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
        //not really need
        rectTransform.DOKill(true);

        if (curState == CurState.HIDE) return; 
        curState = CurState.HIDE;

        rectTransform.DOAnchorPosY(-80, time);
    }
    public void MoveUp(float time)
    {
        rectTransform.DOKill(true);

        if (curState == CurState.UP) return;

        curState = CurState.UP;

        //not really need

        rectTransform.DOAnchorPosY(0, time);
    }
    public void MoveDown(float time)
    {
        rectTransform.DOKill(true);

        if (curState == CurState.DOWN) return;
        curState = CurState.DOWN;

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

    public void Twinkle()
    {
        Debug.Log("Twinkle");
        whiteMask.gameObject.SetActive(true);

        c = StartCoroutine(WaitToSetFalse());
    }
    private IEnumerator WaitToSetFalse()
    {
        yield return new WaitForSeconds(0.1f);

        whiteMask.gameObject.SetActive(false);
    }

    public void Shake()
    {
        var r = image.GetComponent<RectTransform>();
        r.DOKill(true);

        r.DOShakeAnchorPos(1.4f, 80.0f, 10, 10);
    }
    public void Tremble()
    {
        var r = image.GetComponent<RectTransform>();
        s.Kill(true);

        s = DOTween.Sequence();

        s.Append(r.DOShakeAnchorPos(1.8f, new Vector2(20f, 0), 4, 0, false, true, ShakeRandomnessMode.Harmonic));
        s.Join(r.DOAnchorPosY(-50f, 1.8f).SetEase(Ease.OutCirc));
        s.Append(r.DOAnchorPos(new Vector2(0, 0), 1.0f).SetEase(Ease.OutCirc));


    }

    public void KillAllAnim()
    {
        image.GetComponent<RectTransform>().DOKill(true);
        image.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        rectTransform.DOKill(true);

        if (c!=null)
            StopCoroutine(c);
        whiteMask.gameObject.SetActive(false);

        s.Kill(true);
        
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

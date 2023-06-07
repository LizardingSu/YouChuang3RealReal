using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRandom : MonoBehaviour
{
    public RectTransform r;
    public Button button;

    [Range(10,10000)]
    public float strength = 100;
    [Range(1, 100)]
    public int vibrato = 10;
    [Range(0, 10000)]
    public float randomness = 90;

    Sequence s;

    public void Awake()
    {
       s = DOTween.Sequence();

        button.onClick.AddListener(Shake);
    }

    public void OnDestroy()
    {
        button.onClick.RemoveListener(Shake);
    }

    public void Shake()
    {
        s.Kill(true);

        s = DOTween.Sequence();

        s.Append(r.DOShakeAnchorPos(1.8f, new Vector2(20f, 0), 4, 0,false,true,ShakeRandomnessMode.Harmonic));
        s.Join(r.DOAnchorPosY(-50f, 1.8f).SetEase(Ease.OutCirc));
        s.Append(r.DOAnchorPos(new Vector2(0, 0), 1.0f).SetEase(Ease.OutCirc));

    }
}

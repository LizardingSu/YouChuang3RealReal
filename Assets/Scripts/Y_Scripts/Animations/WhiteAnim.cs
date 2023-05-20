using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class WhiteAnim : MonoBehaviour
{
    public Material material;
    public RawImage image;
    public float rollingSpeed = 0.5f;
    public float neededTime = 1f;
    public float mask = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        if (material)
            material.SetFloat("_MyMask", 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float curPos = Time.fixedTime * rollingSpeed;

        material.SetFloat("_MyMask", mask);
        material.mainTextureOffset = new Vector2(curPos * 1280.0f / 908.0f, curPos);

        if (mask <= 0.02f) image.color = new Color(1,1,1,0);
        else image.color = new Color(1,1,1,1);
    }

    public void FadeAndReFill(bool isFade)
    {
        var curMask = material.GetFloat("_MyMask");

        if (isFade)
        {
            if (curMask == 0) return;
        }
        else
        {
            //最大长度：0.75
            if (curMask == 0.75f) return;
        }

        BeginFadeOrReFill(isFade);
    }

    private void BeginFadeOrReFill(bool isFade)
    {
        float end = isFade ? 0 : 0.75f;

        DOTween.To(()=> mask,x => mask = x,end,neededTime);
      
    }
}

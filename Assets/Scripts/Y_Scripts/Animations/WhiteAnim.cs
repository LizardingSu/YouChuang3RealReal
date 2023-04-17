using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class WhiteAnim : MonoBehaviour
{
    public Material material;
    public float rollingSpeed = 0.5f;
    public float fadeSpeed = 0.2f;
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

        material.mainTextureOffset = new Vector2(curPos * 1280.0f / 908.0f, curPos);
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
            //最大长度：0.7
            if (curMask == 0.75f) return;
        }

        StartCoroutine(BeginFadeOrReFill(isFade));
    }

    private IEnumerator BeginFadeOrReFill(bool isFade)
    {
        float end = isFade ? 0 : 0.75f;
        float current = isFade ? 0.75f : 0;
        if (isFade)
        {
            while (current >= end)
            {
                if (current < 0)
                {
                    current = 0;
                    
                }

                yield return new WaitForFixedUpdate();
                material.SetFloat("_MyMask", current);
                current = current - fadeSpeed;
            }
        }
        else
        {
            while (current <= end)
            {
                if (current > 0.75f) current = 0.75f;
                yield return new WaitForFixedUpdate();
                material.SetFloat("_MyMask", current);
                current = current + fadeSpeed;
            }
        }
      
    }
}

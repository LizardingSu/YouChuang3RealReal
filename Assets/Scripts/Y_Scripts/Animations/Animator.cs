using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Animator : MonoBehaviour
{
    public Material material;
    public float rollingSpeed = 0.5f;
    public float fadeSpeed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float curPos = Time.time * rollingSpeed;

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
            if (curMask == 0.7f) return;
        }

        StartCoroutine(BeginFadeOrReFill(isFade));
    }

    private IEnumerator BeginFadeOrReFill(bool isFade)
    {
        float end = isFade ? 0 : 0.7f;
        float current = isFade ? 0.7f : 0;
        if (isFade)
        {
            while (current >= end)
            {
                if (current < 0)
                {
                    current = 0;
                    
                }

                    yield return null;
                material.SetFloat("_MyMask", current);
                current = current - fadeSpeed;
            }
        }
        else
        {
            while (current <= end)
            {
                if (current > 0.7f) current = 0.7f;
                yield return null;
                material.SetFloat("_MyMask", current);
                current = current + fadeSpeed;
            }
        }
      
    }
}

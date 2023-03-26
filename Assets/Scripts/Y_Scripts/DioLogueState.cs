using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DioLogueState : MonoBehaviour
{
    //≤ª∫√Àµ
    public enum State
    {
        Normal,
        Auto,
        FastForward
    }

    public List<string> textList = new List<string>();

    public void Awake()
    {
        //test
        Init(0, "Text/test");
        Debug.Log("Fuck");
    }

    public void OnDestroy()
    {
        textList.Clear();
    }

    public void Init(uint day,string path)
    {
        if(textList.Count!=0) textList.Clear();

        var textAsset = Resources.Load<TextAsset>(path+day);
        var ta = textAsset.text.Split('\n');

        for(int i = 0; i < ta.Length; i++)
        {
            textList.Add(ta[i]);
        }
    }
}

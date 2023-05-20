using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    //好的做法是池化，但是我懒
    public DioLogueState m_dioState;
    public S_AudioManager m_audioManager;


    public bool isComplete = false;

    private AudioClip m_BGM;

    public Image m_Image;

    void Awake()
    {
        m_dioState.dialogueChanged.AddListener(DiologueChanged);
        m_dioState.dialogueWillChange.AddListener(DiologueWillChange);

        m_dioState.isComplete += IsComplete;
    }


    void OnDestroy()
    {
        m_dioState.dialogueChanged.RemoveListener(DiologueChanged);
        m_dioState.dialogueWillChange.RemoveListener(DiologueWillChange);

        m_dioState.isComplete -= IsComplete;
    }

    private void DiologueWillChange(DiologueData data1,DiologueData data2)
    {
        var resources1 = LogEntryParser.GetResourceTypeAndResource(data1.resource);
        var resources2 = LogEntryParser.GetResourceTypeAndResource(data2.resource);

        isComplete = true;

        foreach (var re in resources1)
        {
            if (re.resourcePlace == ResourcePlace.After)
            {
                if (re.resourceType == ResourceType.SE)
                {
                    //如果是自动跳转，则不播放音效
                    if (m_dioState.state != DioState.Auto)
                    {
                        isComplete = false;
                        PlaySE(re.path);
                    }
                }
                else if(re.resourceType == ResourceType.CG)
                {
                    m_Image.color = new Color(1, 1, 1, 0);
                }
            }
        }
        foreach (var re in resources2)
        {
            if (re.resourcePlace == ResourcePlace.Before)
            {
                if (re.resourceType == ResourceType.SE)
                {
                    //同理
                    if (m_dioState.state != DioState.Auto)
                    {
                        isComplete = false;
                        PlaySE(re.path);
                    }
                }
            }
        }

    }

    private void PlaySE(string path)
    {
        var a = Resources.Load("Sounds/" + path) as AudioClip;
        m_audioManager.PlaySE(a);

        StartCoroutine(WaitForSoundsComplete(a.length));
    }

    private IEnumerator WaitForSoundsComplete(float time)
    {
        yield return new WaitForSeconds(time+0.1f);
        isComplete = true;
    }

    private void DiologueChanged(DiologueData data)
    {
        var resources = LogEntryParser.GetResourceTypeAndResource(data.resource);

        if (resources.Count == 0)
            return;

        foreach (var re in resources)
        {
            if(re.resourceType == ResourceType.BGM)
            {
                m_BGM = Resources.Load("BGM/" +re.path) as AudioClip;
                m_audioManager.PlayBGM(m_BGM);
            }
            else if(re.resourceType == ResourceType.CG)
            {
                var tex = Resources.Load("CG/" + re.path) as Texture2D;
                m_Image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                m_Image.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private bool IsComplete()
    {
        return isComplete;
    }

    public void KillAllAnim()
    {
        
    }
}

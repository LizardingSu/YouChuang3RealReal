using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ResourceController : MonoBehaviour
{
    //好的做法是池化，但是我懒
    public DioLogueState m_dioState;
    public S_AudioManager m_audioManager;

    public VideoPlayer m_videoPlayer;
    public RenderTexture m_videoTexture;

    public Sequence s;

    public bool isComplete = false;

    private AudioClip m_BGM;
    private VideoClip m_VideoClip;

    public RawImage m_Image;

    void Awake()
    {
        m_dioState.dialogueChanged.AddListener(DiologueChanged);
        m_dioState.dialogueWillChange.AddListener(DiologueWillChange);

        m_dioState.isComplete += IsComplete;
        m_Image.color = new Color(1, 1, 1, 0);
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
                    if (m_dioState.state != DioState.Auto)
                    {
                        isComplete = false;
                        TakeCG(re.path,false);
                    }
                    else
                    {
                        isComplete = true;
                        TakeCG(re.path);
                    }
                }
                else if(re.resourceType == ResourceType.VIDEO)
                {
                    StopVideo();
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
                else if (re.resourceType == ResourceType.CG)
                {
                    if (m_dioState.state != DioState.Auto)
                    {
                        isComplete = false;
                        PlayCG(re.path, false);
                    }
                    else
                    {
                        isComplete = true;
                        PlayCG(re.path);
                    }
                }
            }
        }

    }

    private void PlayCG(string path,bool isAuto = true)
    {
        var tex = Resources.Load("CG/" + path.Split('.')[0]) as Texture2D;
        m_Image.texture = tex;
        m_Image.color = new Color(1, 1, 1, 1);
        m_Image.rectTransform.anchoredPosition = Vector2.zero;

        if (!isAuto)
        {
            m_Image.rectTransform.anchoredPosition = new Vector2(0,908.0f);
            s = DOTween.Sequence();

            s.Append(m_Image.rectTransform.DOAnchorPosY(0, 1.5f));
            s.AppendInterval(0.5f).AppendCallback(() =>
            {
                isComplete = true;
            });
        }
    }
    private void TakeCG(string path,bool isAuto = true)
    {
        m_Image.rectTransform.anchoredPosition = Vector2.zero;

        if(!isAuto)
        {
            m_Image.rectTransform.anchoredPosition = new Vector2(0, 0.0f);
            s = DOTween.Sequence();

            s.Append(m_Image.rectTransform.DOAnchorPosY(908.0f, 1.5f));
            s.AppendInterval(0.5f).AppendCallback(() =>
            {
                isComplete = true;
                m_Image.color = new Color(1, 1, 1, 0);
            });
        }
        else
        {
            m_Image.color = new Color(1, 1, 1, 0);
        }
    }

    private void PlaySE(string path)
    {
        path = path.Split('.')[0];
        var a = Resources.Load("Sounds/" + path) as AudioClip;
        m_audioManager.PlaySE(a);

        StartCoroutine(WaitForSoundsComplete(a.length));
    }
    private IEnumerator WaitForSoundsComplete(float time)
    {
        yield return new WaitForSeconds(time+0.5f);
        isComplete = true;
    }

    private void PlayVideo(string path)
    {
        m_VideoClip = Resources.Load("CG" + path.Split('.')[0]) as VideoClip;
        m_Image.color = new Color(1, 1, 1, 1);

        //播放前先释放
        m_videoTexture.Release();
        m_videoTexture.Create();

        m_Image.texture = m_videoTexture;
        m_videoPlayer.clip = m_VideoClip;

        m_videoPlayer.Play();
    }
    private void StopVideo()
    {
        m_Image.texture = null;
        m_Image.color = new Color(1, 1, 1, 0);
        m_videoPlayer.Pause();
    }

    private void DiologueChanged(DiologueData data)
    {
        var resources = LogEntryParser.GetResourceTypeAndResource(data.resource);

        if (resources.Count == 0)
            return;

        foreach (var re in resources)
        {
            if(re.resourcePlace == ResourcePlace.In)
            {
                if (re.resourceType == ResourceType.BGM)
                {
                    m_BGM = Resources.Load("BGM/" + re.path.Split('.')[0]) as AudioClip;
                    m_audioManager.PlayBGM(m_BGM);
                }
                else if (re.resourceType == ResourceType.CG)
                {
                    var tex = Resources.Load("CG/" + re.path.Split('.')[0]) as Texture2D;
                    m_Image.texture = tex;
                    m_Image.color = new Color(1, 1, 1, 1);
                }
                else if(re.resourceType == ResourceType.VIDEO)
                {
                    PlayVideo(re.path);
                }
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

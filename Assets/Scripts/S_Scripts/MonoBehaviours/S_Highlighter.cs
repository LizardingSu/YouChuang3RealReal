using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_Highlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material HighlightMat;
    public S_CentralAccessor accessor;

    private void Start()
    {
        accessor = GameObject.Find("MainManager").GetComponent<S_CentralAccessor>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (accessor.GameManager.GamePlaying)
            GetComponent<Image>().material = HighlightMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            GetComponent<Image>().material = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_Highlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material HighlightMat;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
            GetComponent<Image>().material = HighlightMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            GetComponent<Image>().material = null;
    }
}

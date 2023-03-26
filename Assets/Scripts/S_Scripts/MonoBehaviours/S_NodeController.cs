using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_NodeController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
}

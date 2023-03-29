using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_NodeController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite DefaultNode;
    public Sprite ActiveNode;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().sprite = ActiveNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(ActiveNode.rect.width, ActiveNode.rect.height);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<Image>().sprite = DefaultNode;
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DefaultNode.rect.width, DefaultNode.rect.height);
    }
}

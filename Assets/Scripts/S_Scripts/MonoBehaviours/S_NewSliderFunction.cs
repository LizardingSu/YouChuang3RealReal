using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//挂载到ClickArea上
public class S_NewSliderFunction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool BGMSlider;

    [HideInInspector]
    public float MaskOriginWidth;

    [HideInInspector]
    public S_CentralAccessor accessor;

    private RectTransform MaskRT;

    private RectTransform FillRT;

    private bool IsDragging;

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
        MaskRT = transform.parent.Find("FillMask").GetComponent<RectTransform>();
        FillRT = transform.parent.Find("FillMask/Fill").GetComponent<RectTransform>();
        MaskOriginWidth = MaskRT.sizeDelta.x;
        accessor = GameObject.Find("MainManager").GetComponent<S_CentralAccessor>();
        IsDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDragging = false;
    }

    private void Update()
    {
        if (IsDragging)
        {
            DraggingFunction();
        }
    }

    private void DraggingFunction()
    {
        //Debug.Log("Drag");
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 LeftDownPos = corners[0];

        //Debug.Log("鼠标" + Input.mousePosition.ToString());
        //Debug.Log("左下" + corners[0].ToString());

        float newWidth = mousePos.x - LeftDownPos.x;
        newWidth = newWidth < 0 ? 0 : newWidth;
        newWidth = newWidth > MaskOriginWidth ? MaskOriginWidth : newWidth;

        MaskRT.sizeDelta = new Vector2(newWidth, MaskRT.sizeDelta.y);

        float value = newWidth / MaskOriginWidth;
        if (BGMSlider)
        {
            accessor.AudioManager.SetBGMVolume(value);
        }
        else
        {
            accessor.AudioManager.SetSEVolume(value);
        }
    }
}

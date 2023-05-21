using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_FillMaskFunction : MonoBehaviour
{
    public bool BGMSlider;


    //方块初始width
    public float maskOriginWidth;

    //是否正在拖拽
    private bool dragging;

    //accessor
    private S_CentralAccessor accessor;

    private void Awake()
    {
        maskOriginWidth = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        dragging = false;
        accessor = GameObject.Find("MainManager").GetComponent<S_CentralAccessor>();
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
    }

    private void Update()
    {
        if (dragging)
        {
            DraggingFunction();
        }
    }

    private void DraggingFunction()
    {
        Debug.Log("Drag");
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        transform.GetChild(0).GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 LeftDownPos = corners[0];

        //Debug.Log("鼠标" + Input.mousePosition.ToString());
        //Debug.Log("左下" + corners[0].ToString());

        float newWidth = mousePos.x - LeftDownPos.x;
        newWidth = newWidth < 0 ? 0 : newWidth;
        newWidth = newWidth > maskOriginWidth ? maskOriginWidth : newWidth;

        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

        float value = newWidth / maskOriginWidth;
        if (BGMSlider)
        {
            accessor.AudioManager.SetBGMVolume(value);
        }
        else
        {
            accessor.AudioManager.SetSEVolume(value);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}

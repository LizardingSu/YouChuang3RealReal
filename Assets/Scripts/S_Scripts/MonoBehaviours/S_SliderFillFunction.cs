using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class S_SliderFillFunction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool BGMSlider;


    //Mask初始width
    public float maskOriginWidth;

    //是否正在拖拽
    private bool dragging;

    //accessor
    private S_CentralAccessor accessor;

    //private void Start()
    //{
    //    maskOriginWidth = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
    //    dragging = false;
    //    accessor = GameObject.Find("MainManager").GetComponent<S_CentralAccessor>();
    //}

    private void Awake()
    {
        maskOriginWidth = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        dragging = false;
        accessor = GameObject.Find("MainManager").GetComponent<S_CentralAccessor>();
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
        transform.parent.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 LeftDownPos = corners[0];

        //Debug.Log("鼠标" + Input.mousePosition.ToString());
        //Debug.Log("左下" + corners[0].ToString());

        float newWidth = mousePos.x - LeftDownPos.x;
        newWidth = newWidth < 0 ? 0 : newWidth;
        newWidth = newWidth > maskOriginWidth ? maskOriginWidth : newWidth;

        transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, transform.parent.GetComponent<RectTransform>().sizeDelta.y);

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

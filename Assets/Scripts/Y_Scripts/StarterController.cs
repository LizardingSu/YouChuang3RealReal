using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarterController : MonoBehaviour
{
    public List<Image> maskList;
    public DioLogueState dioState;

    private void Awake()
    {
        foreach(Transform child in this.transform)
        {
            maskList.Add(child.gameObject.GetComponent<Image>());
            child.gameObject.SetActive(false);
        }
    }

    private void onDestroy()
    {
        maskList.Clear();
    }
}

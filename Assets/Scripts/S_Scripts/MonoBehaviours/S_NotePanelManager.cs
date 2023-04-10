using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_NotePanelManager : MonoBehaviour
{
    public GameObject NoteScene;

    public Material OutlineMaterial;

    private Dictionary<string, GameObject> ElementsInNote = new Dictionary<string, GameObject>();

    public void SwitchOutline(GameObject g)
    {
        if (g.GetComponent<Image>().material != null)
        {
            g.GetComponent<Image>().material = null;
        }
        else
        {
            g.GetComponent<Image>().material = OutlineMaterial;
        }
    }

    private void Awake()
    {
        for (int i = 0; i < NoteScene.transform.childCount; i++)
        {
            ElementsInNote.Add(NoteScene.transform.GetChild(i).gameObject.name, NoteScene.transform.GetChild(i).gameObject);
        }

        foreach(var element in ElementsInNote)
        {
            element.Value.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}

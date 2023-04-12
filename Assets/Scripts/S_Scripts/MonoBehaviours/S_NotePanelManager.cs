using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_NotePanelManager : MonoBehaviour
{
    public GameObject NoteScene;

    public Material OutlineMaterial;

    public S_CentralAccessor Accessor;

    [HideInInspector]
    public GameObject CurrentActiveItem = null;

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

    public void ItemClickedInNotePanel(S_ItemWithInfo item)
    {
        if (CurrentActiveItem == null)
        {
            NoteScene.transform.Find("BlackMask").gameObject.SetActive(false);
            NoteScene.transform.Find("DialogBoxes").gameObject.SetActive(false);
        }
        else
        {
            CurrentActiveItem.GetComponent<Image>().material = null;
        }

        CurrentActiveItem = NoteScene.transform.Find(item.ToString()).gameObject;
        CurrentActiveItem.GetComponent<Image>().material = OutlineMaterial;
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

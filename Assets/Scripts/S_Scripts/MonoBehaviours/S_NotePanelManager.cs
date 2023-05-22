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

    public void CancelSelect()
    {
        if (CurrentActiveItem != null)
        {
            NoteScene.transform.Find("BlackMask").gameObject.SetActive(true);
            NoteScene.transform.Find("DialogBoxes").gameObject.SetActive(true);
            NoteScene.transform.Find("Buttons").gameObject.SetActive(true);
            transform.Find("DefaultText").gameObject.SetActive(true);
            transform.Find("InfoBox").gameObject.SetActive(false);
            CurrentActiveItem.GetComponent<Image>().material = null;
            CurrentActiveItem = null;
        }
    }

    public void ItemClickedInNotePanel(S_ItemWithInfo item)
    {
        if (CurrentActiveItem == null)
        {
            NoteScene.transform.Find("BlackMask").gameObject.SetActive(false);
            NoteScene.transform.Find("DialogBoxes").gameObject.SetActive(false);
            NoteScene.transform.Find("Buttons").gameObject.SetActive(false);
            transform.Find("DefaultText").gameObject.SetActive(false);
            transform.Find("InfoBox").gameObject.SetActive(true);
        }
        else
        {
            CurrentActiveItem.GetComponent<Image>().material = null;
        }

        CurrentActiveItem = NoteScene.transform.Find(item.ToString()).gameObject;
        CurrentActiveItem.GetComponent<Image>().material = OutlineMaterial;

        string[] discription = null;
        foreach (var i in Accessor.DataManager.ItemInfo.ItemInfoList1)
        {
            if (i.Item == item)
            {
                discription = i.Info.Split('/');
                break;
            }
        }

        GameObject ItemNameText = transform.Find("InfoBox").Find("ItemName").gameObject;
        GameObject ItemInfoText = transform.Find("InfoBox").Find("ItemInfo").gameObject;

        ItemNameText.GetComponent<Text>().text = discription[0];
        ItemInfoText.GetComponent<Text>().text = "";
        for (int i = 1; i < discription.Length; i++)
        {
            ItemInfoText.GetComponent<Text>().text += discription[i] + '\n' + '\n';
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
            if (element.Value.GetComponent<Image>() != null)
            element.Value.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}

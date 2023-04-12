using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_ItemClickedInNote : MonoBehaviour, IPointerClickHandler
{
    public S_ItemWithInfo ThisItem;

    public S_NotePanelManager notePanelManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (notePanelManager.Accessor.StateManager.State == PlaySceneState.Note)
        notePanelManager.ItemClickedInNotePanel(ThisItem);
    }
}

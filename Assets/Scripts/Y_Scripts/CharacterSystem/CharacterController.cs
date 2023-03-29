using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public S_CentralAccessor m_central;

    private DioLogueState m_diologueState;




    private void Awake()
    {
        m_diologueState = m_central._DioLogueState;
        m_diologueState.dialogueChanged.AddListener(ChangeCharacters);
    }

    private void OnDestroy()
    {
        m_diologueState.dialogueChanged.RemoveListener(ChangeCharacters);
    }

   private void ChangeCharacters(DiologueData data)
    {

    }
}

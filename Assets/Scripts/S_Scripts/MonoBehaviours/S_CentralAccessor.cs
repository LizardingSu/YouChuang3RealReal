using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CentralAccessor : MonoBehaviour
{
    [Header("Managers")]
    public S_StateManager StateManager;
    public S_AudioManager AudioManager;
    public S_ProcessManager ProcessManager;
    public S_DataManager DataManager;
    public S_MenuManager MenuManager;
    public S_CoffeeGame GameManager;

    public DioLogueState _DioLogueState;

    [Header("GameObject")]
    public GameObject TransAnim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            MenuManager.NewGame();
        }
    }
}

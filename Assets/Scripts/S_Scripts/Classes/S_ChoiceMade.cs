using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_ChoiceMade
{
    public int ID;
    public int Choice;

    public S_ChoiceMade()
    {
        ID = -1;
        Choice = -1;
    }

    public S_ChoiceMade(int id, int choice)
    {
        ID = id;
        Choice = choice;
    }
}

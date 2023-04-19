using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ChoiceMade
{
    public int ID;
    public string Choice;

    public S_ChoiceMade()
    {
        ID = 0;
        Choice = string.Empty;
    }

    public S_ChoiceMade(int id, string choice)
    {
        ID = id;
        Choice = choice;
    }
}

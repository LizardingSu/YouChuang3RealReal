using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_ChoiceMade
{
    public int ID;
    public int Choice;
    public string Answer;

    public S_ChoiceMade()
    {
        ID = -1;
        Choice = -1;
        Answer = "";
    }

    public S_ChoiceMade(int id, int choice, string answer)
    {
        ID = id;
        Choice = choice;
        Answer = answer;
    }
}

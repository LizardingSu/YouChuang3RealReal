using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_NodeInDay
{
    public S_DayInCalendar WhichDay { get; private set; }
    public int ID { get; private set; }
    public string Description { get; private set; }
    public int Location { get; set; }

    public bool IsCorrect { get; private set; }

    public S_NodeInDay()
    {
        WhichDay = null;
        ID = 0;
        Description = string.Empty;
        Location = 0;
        IsCorrect = false;
    }

    public S_NodeInDay(S_DayInCalendar whichDay, int id, string description, int location, bool isCorrect)
    {
        WhichDay = whichDay;
        ID = id;
        Description = description;
        Location = location;
        IsCorrect = isCorrect;
    }

    public float CalculateLocationInFloat() => (float)Location / (float)WhichDay.TotalConversation;
}
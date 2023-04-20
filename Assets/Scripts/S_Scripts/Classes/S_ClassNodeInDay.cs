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

    public S_NodeInDay()
    {
        WhichDay = null;
        ID = 0;
        Description = string.Empty;
        Location = 0;
    }

    public S_NodeInDay(S_DayInCalendar whichDay, int id, string description, int location)
    {
        WhichDay = whichDay;
        ID = id;
        Description = description;
        Location = location;
    }

    public float CalculateLocationInFloat() => (float)Location / (float)WhichDay.TotalConversation;
}
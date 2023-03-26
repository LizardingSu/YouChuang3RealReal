using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DayInCalendar
{
    public List<S_NodeInDay> Nodes;

    public int TotalConversation { get; private set; }

    public S_DayInCalendar()
    {
        Nodes = new List<S_NodeInDay>();
        TotalConversation = 0;
    }

    public S_DayInCalendar(IEnumerable<S_NodeInDay> nodes, int totalConversation)
    {
        Nodes = new List<S_NodeInDay>();

        foreach (var node in nodes)
        {
            Nodes.Add(node);
        }

        TotalConversation = totalConversation;
    }
}

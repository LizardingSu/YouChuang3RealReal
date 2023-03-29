using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CData
{
    public uint id;
    public string name;
    public CData(uint id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
public class ConfigCharacterFile : ScriptableObject
{
    [SerializeField]
    public List<CData> characterList;
}
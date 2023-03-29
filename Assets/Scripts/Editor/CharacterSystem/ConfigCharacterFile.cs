using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public uint charID;
    public string name;
    public string imgeID;
}

[CreateAssetMenu(menuName = "CharacterFile/CharacterFile", fileName = "CharacterFile")]
public class ConfigCharacterFile : ScriptableObject
{
    [SerializeField]
     List<CharacterData> characterList;

}


public class ExcelImportHelper
{

}
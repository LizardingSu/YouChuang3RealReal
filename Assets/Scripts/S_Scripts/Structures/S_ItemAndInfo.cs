using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct S_ItemAndInfo
{
    [SerializeField]
    S_ItemWithInfo Item;
    [SerializeField]
    string Info;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct S_ItemAndInfo
{
    [SerializeField]
    public S_ItemWithInfo Item;
    [SerializeField]
    public string Info;
}

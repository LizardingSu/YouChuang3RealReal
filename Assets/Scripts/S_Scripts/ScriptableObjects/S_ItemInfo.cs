using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/物品信息")]
public class S_ItemInfo : ScriptableObject
{
    [SerializeField]
    private S_ItemAndInfo[] ItemInfoList;

    public S_ItemAndInfo[] ItemInfoList1 { get => ItemInfoList; set => ItemInfoList = value; }
}
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum S_Steps
{
    Grind = 0,
    Extract,
    ColdWater,
    HotWater,
    Ice,
    Milks,
    CAVA
}

[Serializable]
public class S_Coffee
{
    public string Name;
    public List<S_Steps> Steps;
    public List<S_SpriteForChanging> Sprites;
}

[Serializable]
public class S_SpriteForChanging
{
    public int ChangeStep;
    public Sprite Pic;
}

[CreateAssetMenu(menuName = "ScriptableObject/øß∑»”Œœ∑")]
public class S_CoffeeStep : ScriptableObject
{
    public List<S_Coffee> Coffees;

    public List<string> StepDescription;
}

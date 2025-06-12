using System;
using UnityEngine;

[Serializable]
public struct EffectStackingBehaviour {
    [Min(1)] public int maxStack;

    [Tooltip("Create new effect instance instead of stacking with existing effect")]
    public bool createNewInstance;

    [Tooltip("Reset timer of existing effect with same id")]
    public bool resetTimer;

    [Tooltip("Increase stack count of existing effect with same id")]
    public bool increaseStackCount;

    [Tooltip("Power will be changed correspond to stack count")]
    public bool stackAffectPower;

    [Tooltip("One stack will be taken instead of removing effect when it is end tick")]
    public bool useStackForLifeTime;
}
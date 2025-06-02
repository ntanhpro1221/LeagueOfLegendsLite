using System;

[Serializable]
public struct ItemActiveCost {
    public float_Q3 health;
    public float_Q3 mana;

    public readonly bool IsEnough(in ActiveItemCostSourceAspect source) =>
        health <= source.Health
     && (mana == 0 || (source.IsValid_Mana && mana <= source.Mana));

    public readonly void ApplyCost(in ActiveItemCostSourceAspect source) {
        source.Health -= health;
        if (mana != 0) source.Mana -= mana;
    }
}
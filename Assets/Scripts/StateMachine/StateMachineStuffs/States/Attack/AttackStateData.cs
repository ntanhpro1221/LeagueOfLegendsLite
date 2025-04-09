using Unity.Entities;
using Unity.NetCode;

public struct AttackStateData : IComponentData {
    [GhostField] public NetworkTick cooldownDoneAtTick;
    [GhostField] public NetworkTick realAttackAtTick;
    [GhostField] public bool        isAttacked;

    public void MarkAttacked() => isAttacked = true;

    public void ResetCooldown() => cooldownDoneAtTick = NetworkTick.Invalid;
    
    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    /// <summary>
    /// It's time to actually deal damage
    /// </summary>
    public readonly bool IsAttackReady(in NetworkTick curTick) => 
        !isAttacked 
     && curTick.IsNewerThan(realAttackAtTick);

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    /// <summary>
    /// Ready for the next attack
    /// </summary>
    public readonly bool IsCooldownDone(in NetworkTick curTick) =>
        !cooldownDoneAtTick.IsValid
     || curTick.IsNewerThan(cooldownDoneAtTick);
}
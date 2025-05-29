using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputPrevCode : IComponentData {
    [GhostField] public PlayerTrigger.Item<int> Code;
}

public struct PlayerInputData : IInputComponentData {
    public void ResetAllEvents() {
        triggers.Event = default;
    }

#region TRIGGERS

    [GhostField] public TickVersionForInput     tickVersion;
    [GhostField] public PlayerTrigger.Full triggers;

    public readonly bool GetFullWithTick(
        PlayerTrigger.Key          key
      , ref PlayerInputPrevCode prevCode
      , in  NetworkTick                 curTick) =>
        tickVersion.IsValid(curTick)
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
     && triggers.GetFull(key, ref prevCode);

#endregion

#region MOVE

    [GhostField] public float3_Q3 moveLocTarget;

    public void SetMove(float3_Q3 _targetLocalPos) {
        moveLocTarget = _targetLocalPos;
        triggers.Set(PlayerTrigger.Key.Move);
    }

    public void CancelMove() {
        triggers.Set(PlayerTrigger.Key.CancelMove);
    }

#endregion

#region ATTACK

    [GhostField] public Entity attackTarget;

    public void SetAttack(Entity target) => attackTarget = target;

    public void CancelAttack() => attackTarget = Entity.Null;

#endregion
}

[GhostEnabledBit]
public struct PlayerInputResetting : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(NormalAttackableAuthoring))]
public class PlayerInputAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<PlayerInputPrevCode>(entity);
            AddComponent<PlayerInputData>(entity);
            AddComponent<PlayerInputResetting>(entity);
        }
    }
}
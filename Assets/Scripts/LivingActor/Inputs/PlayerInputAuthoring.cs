using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputData : IInputComponentData {
    public void ResetAllEvents() {
        triggers.Event = default;
    }
    
    [GhostField] public InputForActivableItemData inputForActivableItem;
    [GhostField] public ItemActiveCondition       curCondition;
    [GhostField] public PlayerTrigger.Full        triggers;

#region MOVE

    [GhostField] public float3_Q3 moveLocTarget;

    public void SetMove(float3_Q3 _targetLocalPos) {
        moveLocTarget = _targetLocalPos;
        triggers.Set(PlayerTrigger.Other.Move);
    }

    public void CancelMove() {
        triggers.Set(PlayerTrigger.Other.CancelMove);
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

            AddComponent<PlayerTrigger.PrevCode>(entity);
            AddComponent<PlayerInputData>(entity);
            AddComponent<PlayerInputResetting>(entity);
        }
    }
}
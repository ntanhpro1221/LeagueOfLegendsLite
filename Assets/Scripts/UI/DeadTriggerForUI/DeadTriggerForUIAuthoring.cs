using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct DeadTriggerForUIData : IComponentData {
    public bool prevStateIsDead;
    public bool deadThisFrame;
    public bool respawnThisFrame;

    public void Update(bool curDeadState) {
        deadThisFrame = respawnThisFrame = false;

        if (prevStateIsDead == curDeadState) return;
        prevStateIsDead = curDeadState;

        if (curDeadState)
            deadThisFrame     = true;
        else respawnThisFrame = true;
    }

    public readonly void UpdateHandler(
        DeadHandler_Base           handler
      , in NetworkTick             curTick
      , in DeadStateData           deadData
      , in EnabledRefRO<DeadState> deadState) {
        if (deadThisFrame) handler.Dead(curTick, deadData.respawnAtTick);
        else if (respawnThisFrame) handler.Respawn();
        if (deadState.ValueRO) handler.UpdateDead(curTick);
    }
}

public class DeadTriggerForUIAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<DeadTriggerForUIAuthoring> {
        public override void Bake(DeadTriggerForUIAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            AddComponent<DeadTriggerForUIData>(entity);
        }
    }
}
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

public struct PlayerInputData : IInputComponentData {
    #region GENERAL

    public InputEvent doneResetEvent;

    public void ResetAllEvents() {
        doneResetEvent = new InputEvent();
        moveEvent      = new InputEvent();
    }

    #endregion

    #region MOVE

    [GhostField] public float3_Q3  moveLocTarget;
    [GhostField] public InputEvent moveEvent;

    public void SetMove(float3_Q3 _targetLocalPos) {
        moveLocTarget = _targetLocalPos;
        moveEvent.Set();
    }

    public void CancelMove(in LocalTransform locTrans) {
        moveLocTarget = locTrans.Position.Quantizate3();
        moveEvent       = new InputEvent();
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
    private class Baker : Baker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInputData>(entity);
            AddComponent<PlayerInputResetting>(entity);
        }
    }
}
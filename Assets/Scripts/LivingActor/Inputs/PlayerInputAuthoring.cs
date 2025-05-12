using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputData : IInputComponentData {
#region GENERAL

    private static readonly InputEvent NullEvent = new();

    public InputEvent doneResetEvent;

    public void ResetAllEvents() {
        doneResetEvent  = NullEvent;
        moveEvent       = NullEvent;
        cancelMoveEvent = NullEvent;
    }

#endregion

#region MOVE

    [GhostField] public float3_Q3  moveLocTarget;
    [GhostField] public InputEvent moveEvent;
    [GhostField] public InputEvent cancelMoveEvent;

    public void SetMove(float3_Q3 _targetLocalPos) {
        moveLocTarget = _targetLocalPos;
        moveEvent.Set();
    }

    public void CancelMove() {
        moveEvent = NullEvent;
        cancelMoveEvent.Set();
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
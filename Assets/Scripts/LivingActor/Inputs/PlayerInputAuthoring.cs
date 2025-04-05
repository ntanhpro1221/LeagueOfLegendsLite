using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct PlayerInputData : IInputComponentData {
    #region GENERAL

    
    private static readonly InputEvent TriggeredInputEvent = new() { Count = 1 };
    public                  void       Reset() => this = new PlayerInputData();

    #endregion
    
    #region MOVE

    [GhostField] public float3_Q3 targetLocalPos;
    [GhostField] public InputEvent moveEvent;
    public void SetMove(float3_Q3 _targetLocalPos) {
        moveEvent      = TriggeredInputEvent;
        targetLocalPos = _targetLocalPos;
    }

    #endregion
}

[RequireComponent(typeof(MoveableAuthoring))]
public class PlayerInputAuthoring : MonoBehaviour {
    private class Baker : Baker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInputData>(entity);
        }
    }
}
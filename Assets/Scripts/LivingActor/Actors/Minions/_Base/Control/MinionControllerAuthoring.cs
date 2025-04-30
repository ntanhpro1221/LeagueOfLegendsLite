using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct MinionControlData : IComponentData {
    public bool skibidi_dop_dop_yes_yes;
}

/// <summary>
/// Be initialized in <see cref="InitMinionSystem"/>
/// </summary>
public struct MinionFixedPathBuffer : IBufferElementData {
    [GhostField] public float3_Q3 pos;
}

public class MinionControllerAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<MinionControllerAuthoring> {
        public override void Bake(MinionControllerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<MinionControlData>(entity);
            AddBuffer<MinionFixedPathBuffer>(entity);
        }
    }
}
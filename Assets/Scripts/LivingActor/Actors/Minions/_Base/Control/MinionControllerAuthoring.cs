using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct MinionControlFactor : IComponentData {
    [GhostField] public int aggroRangeSqr;
}

/// <summary>
/// Be initialized in <see cref="InitMinionSystem"/>
/// </summary>
public struct MinionFixedPathBuffer : IBufferElementData {
    [GhostField] public float3_Q3 pos;
}

[GhostEnabledBit]
public struct MinionAggroAnchor : IComponentData, IEnableableComponent {
    [GhostField] public float3_Q3 anchor;
}

[GhostEnabledBit]
public struct MinionAggroDisabling : IComponentData, IEnableableComponent {
    [GhostField] public NetworkTick doneAtTick;
    [GhostField] public int         pathLengthWhenDiable;
}

public class MinionControllerAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<MinionControllerAuthoring> {
        public override void Bake(MinionControllerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<MinionControlFactor>(entity);
            AddBuffer<MinionFixedPathBuffer>(entity);

            AddComponentDisabled<MinionAggroAnchor>(entity);
            AddComponentDisabled<MinionAggroDisabling>(entity);
        }
    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterControlFactor : IComponentData {
    public int  leashRangeSqr;
    public uint respawnCDTick;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterCanRespawn : IComponentData, IEnableableComponent { }

/// <summary>
/// - Turn on when monster be attack by champion.<br/>
/// - During this state, he will trace nearest target until there is no target in his <see cref="MonsterControlFactor.leashRangeSqr"/>.<br/>
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterLeashAnchor : IComponentData, IEnableableComponent {
    public float3_Q3  anchorPos;
    public floatXZ_Q3 anchorDir;

    public static MonsterLeashAnchor FromLocTrans(in LocalTransform locTrans) => new() {
        anchorPos = locTrans.Position.Quantizate3()
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
      , anchorDir = locTrans.Forward().WithoutY().Quantizate3().xz
    };
}

/// <summary>
/// - Turn on when monster has just lost his target (or out of <see cref="MonsterControlFactor.leashRangeSqr"/>).<br/>
/// - Then he will return to his anchor and cannot trace any target during this process.<br/>
/// - This process end when monster reach his anchor.<br/>
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterLeashDisabling : IComponentData, IEnableableComponent {
    public NetworkTick nextRegenTick;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct BeBeaten : IComponentData, IEnableableComponent {
    public Entity source;
}

[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct MonsterDisableHealthRegen : IComponentData, IEnableableComponent { }

public class MonsterControllerAuthoring : MonoBehaviour {
    public bool disableHealthRegen;
    
    private class Baker : ExtendBaker<MonsterControllerAuthoring> {
        public override void Bake(MonsterControllerAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<MonsterControlFactor>(entity);
            
            AddComponentDisabled<MonsterCanRespawn>(entity);

            AddComponentDisabled<MonsterLeashAnchor>(entity);

            AddComponent(entity, new MonsterLeashDisabling { nextRegenTick = new NetworkTick(0) });
            SetComponentEnabled<MonsterLeashDisabling>(entity, false);

            AddComponentDisabled<BeBeaten>(entity);
            
            AddComponent<MonsterDisableHealthRegen>(entity);
            SetComponentEnabled<MonsterDisableHealthRegen>(entity, authoring.disableHealthRegen);
        }
    }
}
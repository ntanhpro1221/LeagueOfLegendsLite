using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct MonsterCommonBehaviourConfigData : IComponentData {
    public float_Q3 hpRegenUnleash_Percent;
    public uint     hpRegenUnleash_IntervalTick;
}

public class MonsterCommonBehaviourConfigAuthoring : MonoBehaviour {
    public NetCodeConfig netConfig;
    public float_Q3      hpRegenUnleash_Percent      = 15;
    public float_Q3      hpRegenUnleash_IntervalTime = 1;

    private class Baker : ExtendBaker<MonsterCommonBehaviourConfigAuthoring> {
        public override void Bake(MonsterCommonBehaviourConfigAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new MonsterCommonBehaviourConfigData {
                hpRegenUnleash_Percent = authoring.hpRegenUnleash_Percent
              , hpRegenUnleash_IntervalTick = TickHelpers.CountTick(
                    authoring.hpRegenUnleash_IntervalTime
                  , authoring.netConfig.ClientServerTickRate.SimulationTickRate
                  , TickHelpers.RoundMethod.Nearest)
            });
        }
    }
}
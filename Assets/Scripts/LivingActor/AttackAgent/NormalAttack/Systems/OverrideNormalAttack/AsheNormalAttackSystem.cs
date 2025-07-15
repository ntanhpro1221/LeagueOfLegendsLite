using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(BeforeNormalAttackSystemGroup))]
public partial struct AsheNormalAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
        state.RequireForUpdate<AsheTag>();
        state.RequireForUpdate<EndPredictedSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndPredictedSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var allItem                        = SystemAPI.GetSingleton<AllItemData>();
        var isFirstTimeFullyPredictingTick = SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick;

        foreach (var data in SystemAPI.Query<UpdateAspect>().WithAll<Simulate>()) {

            var myPos    = data.LocTrans.Position.Quantizate3();
            var personal = data.PersonalConstructor.Construct();

            // Just stack when not activating
            if (!data.EffectMap.ContainsKey(EffectId.AsheSkill_Q_Active)) {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                data.IncomingEffect.Add(new IncomingEffectBuffer {
                    id           = new EffectFullId { id = EffectId.AsheSkill_Q_Stack, source = data.Entity }
                  , senderScaler = personal
                  , senderPos    = myPos
                });

                return;
            }

            data.AttackTrigger.ValueRW = false;

            // Do real spawn once
            if (!isFirstTimeFullyPredictingTick) return;

            var spawnRoot = LocalTransform.FromPositionRotation(
                LocalTransform
                    .FromPositionRotation(data.LocTrans.Position, data.RotationData.quaternion)
                    .TransformPoint(data.ProjSpawnPnt.point.position)
              , data.RotationData.quaternion);
            var triggerSource = new DamageTriggerSource {
                damage = ((float)data.PersonalConstructor.Stats.PhysicDamage / 100 * data.Items
                    .GetItemDataUnsafe(SlotItemId.Skill_Q, allItem).concreteProp
                    .Value[(int)AsheSkill_Q.ConcreteProperty.dmgRatioPerArrow]
                    [data.Items.Slots.Skill_Q.CalcSafeLevelIndex()]).Quantizate3()
              , source       = data.Entity
              , sourcePos    = myPos
              , sourceScaler = personal
            };

            const float deltaPosFactor = 100;
            data.FireProjectile(spawnRoot.Translate(new float3(deltaPosFactor,  0, 0)),               triggerSource, ref ecb);
            data.FireProjectile(spawnRoot.Translate(new float3(-deltaPosFactor, 0, 0)),               triggerSource, ref ecb);
            data.FireProjectile(spawnRoot.Translate(new float3(0,               0, deltaPosFactor)),  triggerSource, ref ecb);
            data.FireProjectile(spawnRoot.Translate(new float3(0,               0, -deltaPosFactor)), triggerSource, ref ecb);
            data.FireProjectile(spawnRoot.Translate(new float3(0,               0, 0)),               triggerSource, ref ecb);
        }
    }

    private readonly partial struct UpdateAspect : IAspect {
        private readonly RefRO<AimedTargetData>      _Target;
        private readonly RefRO<LocalTransform>       _LocTrans;
        private readonly RefRO<ProjectileSpawnPoint> _ProjSpawnPnt;
        private readonly RefRO<RotationData>         _RotationData;

        public ref readonly AimedTargetData      Target       => ref _Target.ValueRO;
        public ref readonly LocalTransform       LocTrans     => ref _LocTrans.ValueRO;
        public ref readonly ProjectileSpawnPoint ProjSpawnPnt => ref _ProjSpawnPnt.ValueRO;
        public ref readonly RotationData         RotationData => ref _RotationData.ValueRO;

        [ReadOnly] public readonly DynamicBuffer<DamageTriggerSource.EffectBuffer> OnHitEffects;
        [ReadOnly] public readonly DynamicBuffer<AsheSkill_Q.PrefabBuffer>         Prefabs;

        public readonly DynamicBuffer<IncomingEffectBuffer> IncomingEffect;
        public readonly ScalerPersonalConstructAspect       PersonalConstructor;
        public readonly EffectMapAspectRO                   EffectMap;
        public readonly ItemSlotsAspectRO                   Items;
        public readonly EnabledRefRW<RangedAttackTrigger>   AttackTrigger;
        public readonly Entity                              Entity;

        public void FireProjectile(
            in  LocalTransform      spawnPnt
          , in  DamageTriggerSource triggerSource
          , ref EntityCommandBuffer ecb) {
            var projectile = ecb.Instantiate(Prefabs[(int)AsheSkill_Q.ConcretePrefab.arrow].entity);

            // Set target
            ecb.SetComponent(projectile, Target);

            // Set transform
            ecb.SetComponent(projectile, spawnPnt);

            // Set damage data
            ecb.SetComponent(projectile, triggerSource);

            // Set on-hit effect
            foreach (var effect in OnHitEffects) ecb.AppendToBuffer(projectile, effect);
        }
    }
}
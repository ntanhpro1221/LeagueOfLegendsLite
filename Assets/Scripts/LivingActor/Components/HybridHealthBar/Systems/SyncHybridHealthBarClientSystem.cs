using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(CameraFollowClientSystem))]
public partial struct SyncHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        var     cam            = Camera.main;
        ref var statsEnumIndex = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        var     healthId       = statsEnumIndex[StatsType.Health];
        var     manaId         = statsEnumIndex[StatsType.Mana];

        foreach (var data in SystemAPI.Query<UpdateAspect>()) {
            data.HybridPos = cam!
                .WorldToScreenPoint(data.LocPos.WithAddY(data.DeltaY))
                .WithoutZ();

            data.UI.UpdateUI(
                maxHealth: data.MaxHealth(healthId)
              , curHealth: data.CurHealth
              , curArmor: 0
              , maxMana: data.MaxMana(manaId)
              , curMana: data.CurMana
              , curLevel: data.CurLevel);
        }

        ecb.Playback(state.EntityManager);
    }

    private readonly partial struct UpdateAspect : IAspect {
        private const float DEFAULT_OPTIONAL_FLOAT = 1;
        private const int   DEFAULT_OPTIONAL_INT   = 1;

        private readonly RefRO<HybridHealthBarData> _HybridData;
        private readonly RefRO<HealthData>          _HealthData;
        private readonly RefRO<LocalTransform>      _LocTrans;

        [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

        [Optional] private readonly RefRO<ManaData>  _ManaData;
        [Optional] private readonly RefRO<LevelData> _LevelData;

        public Vector3 HybridPos {
            set => _HybridData.ValueRO.transRef.Value.position = value;
        }

        public float       DeltaY => _HybridData.ValueRO.deltaY;
        public float3      LocPos => _LocTrans.ValueRO.Position;
        public HealthBarUI UI     => _HybridData.ValueRO.UIRef.Value;

        public float MaxHealth(int healthId) => _Stats[healthId].value;
        public float MaxMana(int   manaId)   => _ManaData.IsValid ? _Stats[manaId].value : DEFAULT_OPTIONAL_FLOAT;

        public float CurHealth => _HealthData.ValueRO.value;
        public float CurMana   => _ManaData.IsValid ? _ManaData.ValueRO.value : DEFAULT_OPTIONAL_FLOAT;
        public int   CurLevel  => _LevelData.IsValid ? _LevelData.ValueRO.curLevel : DEFAULT_OPTIONAL_INT;
    }
}
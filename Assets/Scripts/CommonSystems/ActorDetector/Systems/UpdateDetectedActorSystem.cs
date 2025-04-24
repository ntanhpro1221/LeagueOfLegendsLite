using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct UpdateDetectedActorSystem : ISystem {
    [ReadOnly] private ComponentLookup<ActorDetector> actorDetectorLookup;

    [ReadOnly] private ComponentLookup<ChampionTag> championLookup;
    [ReadOnly] private ComponentLookup<MinionTag>   minionLookup;
    [ReadOnly] private ComponentLookup<TowerTag>    towerLookup;
    [ReadOnly] private ComponentLookup<MonsterTag>  monsterLookup;

    private BufferLookup<DetectedChampionBuffer> detectedChampionLookup;
    private BufferLookup<DetectedMinionBuffer>   detectedMinionLookup;
    private BufferLookup<DetectedTowerBuffer>    detectedTowerLookup;
    private BufferLookup<DetectedMonsterBuffer>  detectedMonsterLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();

        actorDetectorLookup = SystemAPI.GetComponentLookup<ActorDetector>(
            isReadOnly: true);

        championLookup = SystemAPI.GetComponentLookup<ChampionTag>(
            isReadOnly: true);
        minionLookup = SystemAPI.GetComponentLookup<MinionTag>(
            isReadOnly: true);
        towerLookup = SystemAPI.GetComponentLookup<TowerTag>(
            isReadOnly: true);
        monsterLookup = SystemAPI.GetComponentLookup<MonsterTag>(
            isReadOnly: true);

        detectedChampionLookup = SystemAPI.GetBufferLookup<DetectedChampionBuffer>(
            isReadOnly: false);
        detectedMinionLookup = SystemAPI.GetBufferLookup<DetectedMinionBuffer>(
            isReadOnly: false);
        detectedTowerLookup = SystemAPI.GetBufferLookup<DetectedTowerBuffer>(
            isReadOnly: false);
        detectedMonsterLookup = SystemAPI.GetBufferLookup<DetectedMonsterBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new ClearDetectedChampionJob()
            .ScheduleParallel(state.Dependency);
        state.Dependency = new ClearDetectedMinionJob()
            .ScheduleParallel(state.Dependency);
        state.Dependency = new ClearDetectedTowerJob()
            .ScheduleParallel(state.Dependency);
        state.Dependency = new ClearDetectedMonsterJob()
            .ScheduleParallel(state.Dependency);
        state.CompleteDependency();

        actorDetectorLookup.Update(ref state);

        championLookup.Update(ref state);
        minionLookup.Update(ref state);
        towerLookup.Update(ref state);
        monsterLookup.Update(ref state);

        detectedChampionLookup.Update(ref state);
        detectedMinionLookup.Update(ref state);
        detectedTowerLookup.Update(ref state);
        detectedMonsterLookup.Update(ref state);

        state.Dependency = new UpdateNewDetectedActorJob {
            actorDetectorLookup    = actorDetectorLookup
          , championLookup         = championLookup
          , minionLookup           = minionLookup
          , towerLookup            = towerLookup
          , monsterLookup          = monsterLookup
          , detectedChampionLookup = detectedChampionLookup
          , detectedMinionLookup   = detectedMinionLookup
          , detectedTowerLookup    = detectedTowerLookup
          , detectedMonsterLookup  = detectedMonsterLookup
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedChampionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedChampionBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedMinionJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedMinionBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedTowerJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedTowerBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct ClearDetectedMonsterJob : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DetectedMonsterBuffer> detectedBuffer) {
            detectedBuffer.Clear();
        }
    }

    [BurstCompile]
    private partial struct UpdateNewDetectedActorJob : ITriggerEventsJob {
        [ReadOnly] public ComponentLookup<ActorDetector> actorDetectorLookup;

        [ReadOnly] public ComponentLookup<ChampionTag> championLookup;
        [ReadOnly] public ComponentLookup<MinionTag>   minionLookup;
        [ReadOnly] public ComponentLookup<TowerTag>    towerLookup;
        [ReadOnly] public ComponentLookup<MonsterTag>  monsterLookup;

        public BufferLookup<DetectedChampionBuffer> detectedChampionLookup;
        public BufferLookup<DetectedMinionBuffer>   detectedMinionLookup;
        public BufferLookup<DetectedTowerBuffer>    detectedTowerLookup;
        public BufferLookup<DetectedMonsterBuffer>  detectedMonsterLookup;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent) {
            var alice = triggerEvent.EntityA;
            var bob   = triggerEvent.EntityB;

            TryAppendToHolder(alice, bob);
            TryAppendToHolder(bob,   alice);
        }

        [BurstCompile]
        private void TryAppendToHolder(in Entity detectorEntity, in Entity target) {
            if (!actorDetectorLookup.TryGetComponent(detectorEntity, out var detector)
             || detector.holder == target) // Not include itself
                return;

            if (championLookup.HasComponent(target)
             && detectedChampionLookup.TryGetBuffer(detector.holder, out var championHolder))
                championHolder.Add(target);

            if (minionLookup.HasComponent(target)
             && detectedMinionLookup.TryGetBuffer(detector.holder, out var minionHolder))
                minionHolder.Add(target);

            if (towerLookup.HasComponent(target)
             && detectedTowerLookup.TryGetBuffer(detector.holder, out var towerHolder))
                towerHolder.Add(target);

            if (monsterLookup.HasComponent(target)
             && detectedMonsterLookup.TryGetBuffer(detector.holder, out var monsterHolder))
                monsterHolder.Add(target);
        }
    }
}
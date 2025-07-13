using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleBountySystemGroup))]
public partial struct ApplyBountySystem : ISystem {
    private EntityQuery mainQuery;

    private ComponentLookup<KDAData>        kdaLookup;
    private ComponentLookup<CreepScoreData> creepLookup;
    private ComponentLookup<GlobalKDAData>  globalKDALookup;
    private BufferLookup<IncomingExpBuffer> expLookup;

    [ReadOnly] private ComponentLookup<TeamTypeData>   teamLookup;
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<ChampionTag>    champLookup;

    [ReadOnly] private NativeList<Entity> blueTeam;
    [ReadOnly] private NativeList<Entity> redTeam;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CommonGameRulesData>();
        state.RequireForUpdate<GlobalKDAData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , BountyTrigger
            >().Build();

        kdaLookup       = SystemAPI.GetComponentLookup<KDAData>(isReadOnly: false);
        creepLookup     = SystemAPI.GetComponentLookup<CreepScoreData>(isReadOnly: false);
        globalKDALookup = SystemAPI.GetComponentLookup<GlobalKDAData>(isReadOnly: false);
        expLookup       = SystemAPI.GetBufferLookup<IncomingExpBuffer>(isReadOnly: false);

        teamLookup     = SystemAPI.GetComponentLookup<TeamTypeData>(isReadOnly: true);
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(isReadOnly: true);
        champLookup    = SystemAPI.GetComponentLookup<ChampionTag>(isReadOnly: true);

        blueTeam = new NativeList<Entity>(10, Allocator.Persistent);
        redTeam  = new NativeList<Entity>(10, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        kdaLookup.Update(ref state);
        creepLookup.Update(ref state);
        globalKDALookup.Update(ref state);
        expLookup.Update(ref state);

        teamLookup.Update(ref state);
        locTransLookup.Update(ref state);
        champLookup.Update(ref state);

        UpdateTeam(ref state);

        state.Dependency = new Job {
            bountyNearSqr   = SystemAPI.GetSingleton<CommonGameRulesData>().bountyNearSqr
          , globalKDAEntity = SystemAPI.GetSingletonEntity<GlobalKDAData>()

          , kdaLookup       = kdaLookup
          , creepLookup     = creepLookup
          , globalKDALookup = globalKDALookup
          , expLookup       = expLookup

          , teamLookup     = teamLookup
          , locTransLookup = locTransLookup
          , champLookup    = champLookup

          , blueTeam = blueTeam
          , redTeam  = redTeam
        }.Schedule(state.Dependency);
    }

    private void UpdateTeam(ref SystemState state) {
        blueTeam.Clear();
        redTeam.Clear();

        foreach (var (
            team
          , entity
            ) in SystemAPI
            .Query<
                RefRO<TeamTypeData>
            >().WithAll<
                ChampionTag
            >().WithNone<
                DummyTag
            >().WithEntityAccess())
            switch (team.ValueRO.team) {
                case TeamType.Blue: blueTeam.Add(entity); break;
                case TeamType.Red:  redTeam.Add(entity); break;
            }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        blueTeam.Dispose();
        redTeam.Dispose();
    }

    [WithAll(
        typeof(Simulate)
      , typeof(BountyTrigger))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public int    bountyNearSqr;
        public Entity globalKDAEntity;

        public ComponentLookup<KDAData>        kdaLookup;
        public ComponentLookup<CreepScoreData> creepLookup;
        public ComponentLookup<GlobalKDAData>  globalKDALookup;
        public BufferLookup<IncomingExpBuffer> expLookup;

        [ReadOnly] public ComponentLookup<TeamTypeData>   teamLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<ChampionTag>    champLookup;

        [ReadOnly] public NativeList<Entity> blueTeam;
        [ReadOnly] public NativeList<Entity> redTeam;

        private void AddGoldExp(in Entity receiver, float_Q3 gold, float_Q3 exp, in Entity sender, ref DynamicBuffer<OutgoingGoldBuffer> outgoingGolds) {
            if (gold != 0) outgoingGolds.Add(new OutgoingGoldBuffer { gold = gold, target = receiver });
            if (exp  != 0) expLookup[receiver].Add((int)exp);
        }

        [BurstCompile]
        private void Execute(
            in  BountyTriggerData                 bountyTriggerData
          , in  BountyData                        bounties
          , in  LocalTransform                    locTrans
          , in  DynamicBuffer<AssistBuffer>       assists
          , ref DynamicBuffer<OutgoingGoldBuffer> outgoingGolds
          , in  Entity                            entity) {
            var source     = bountyTriggerData.lastHitEntity;
            var sourceTeam = teamLookup[source].team == TeamType.Blue ? blueTeam : redTeam;

            // DIRECT KILL
            AddGoldExp(
                receiver: source
              , gold: bounties.data.Gold_Kill
              , exp: bounties.data.Exp_Kill
              , sender: entity
              , outgoingGolds: ref outgoingGolds);

            // ALL TEAM
            foreach (var teamate in sourceTeam)
                AddGoldExp(
                    receiver: teamate
                  , gold: bounties.data.Gold_Team
                  , exp: bounties.data.Exp_Team
                  , sender: entity
                  , outgoingGolds: ref outgoingGolds);

            // ASSIST
            foreach (var assist in assists) {
                if (assist.entity == source
                 || !champLookup.HasComponent(assist.entity))
                    break;

                AddGoldExp(
                    receiver: assist.entity
                  , gold: bounties.data.Gold_Assist
                  , exp: bounties.data.Exp_Assist
                  , sender: entity
                  , outgoingGolds: ref outgoingGolds);

                kdaLookup.GetRefRW(assist.entity).ValueRW.assist += (int)bounties.data.KillScore;
            }

            // NEAR
            foreach (var teamate in sourceTeam) {
                if (teamate == entity
                 || bountyNearSqr < GameHelpers.DistanceXZ_Sqr(
                        locTrans.Position
                      , locTransLookup[teamate].Position))
                    continue;

                AddGoldExp(
                    receiver: teamate
                  , gold: bounties.data.Gold_Near
                  , exp: bounties.data.Exp_Near
                  , sender: entity
                  , outgoingGolds: ref outgoingGolds);
            }

            // CREEP SCORE
            creepLookup.GetRefRW(source).ValueRW.creepScore += bounties.data.CreepScore;

            // KDA
            int bountyKill = (int)bounties.data.KillScore;
            kdaLookup.GetRefRW(source).ValueRW.kill += bountyKill;

            // GLOBAL KDA
            globalKDALookup.GetRefRW(globalKDAEntity).ValueRW.AddKill(teamLookup[source].team, bountyKill);
        }
    }
}
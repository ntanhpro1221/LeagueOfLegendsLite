using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct ApplyIncomingExpSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RequireExpData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            requireExp = SystemAPI.GetSingleton<RequireExpData>()
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public RequireExpData requireExp;

        [BurstCompile]
        public void Execute(
            ref LevelData                        level
          , ref DynamicBuffer<IncomingExpBuffer> expBuffer) {
            // add exp
            foreach (var exp in expBuffer) level.curExp += exp.exp;
            expBuffer.Clear();

            // level up
            while (level.curLevel < requireExp.MaxLevel) {
                int nextLevelExp = requireExp.CalcRequireExpForNextLevel(level.curLevel);
                if (level.curExp < nextLevelExp) break;
                level.curExp -= nextLevelExp;
                level.curLevel++;
                level.availableSkillPoint++;
            }
        }
    }
}
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
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

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public RequireExpData requireExp;

        [BurstCompile]
        private void Execute(
            ref LevelData                        level
          , in  DynamicBuffer<IncomingExpBuffer> expBuffer) {
            // add exp
            foreach (var exp in expBuffer) level.curExp += exp.exp;

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
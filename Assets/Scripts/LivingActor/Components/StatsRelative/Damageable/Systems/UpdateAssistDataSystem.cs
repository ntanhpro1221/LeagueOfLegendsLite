using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial struct UpdateAssistDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<CommonGameRulesData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        state.Dependency = new UpdateJob {
            newResetAtTick = curTick.WithBonusTick(
                SystemAPI.GetSingleton<CommonGameRulesData>().resetAssistTick)
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new ResetJob {
            curTick = curTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [WithPresent(
        typeof(AssistResetTrigger))]
    [BurstCompile]
    private partial struct UpdateJob : IJobEntity {
        public NetworkTick newResetAtTick;

        [BurstCompile]
        private void Execute(
            ref AssistResetData                     data
          , EnabledRefRW<AssistResetTrigger>        trigger
          , ref DynamicBuffer<AssistBuffer>         assistBuffer
          , in  DynamicBuffer<IncomingDamageBuffer> damageBuffer) {
            if (damageBuffer.IsEmpty) return;
            
            // Mark need reset
            trigger.ValueRW = true;
            
            // Assign reset tick
            data.resetAtTick = newResetAtTick;
            
            // Add new assist elements
            foreach (var damage in damageBuffer) {
                bool existed = false;
                foreach (var assist in assistBuffer)
                    if (damage.source == assist.entity) {
                        existed = true;
                        break;
                    }

                if (!existed) assistBuffer.Add(damage.source);
            } 
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(AssistResetTrigger))]
    [BurstCompile]
    private partial struct ResetJob : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        private void Execute(
            in AssistResetData               data
          , EnabledRefRW<AssistResetTrigger> trigger
          , ref DynamicBuffer<AssistBuffer>  buffer) {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (data.resetAtTick.IsNewerThan(curTick)) return;

            // Mark reset completed
            trigger.ValueRW = false;

            // Do reset
            buffer.Clear();
        }
    }
}
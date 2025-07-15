using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(ActiveItemWithoutStateSystemGroup))]
public partial struct ActiveItemWithoutStateSystem_Ashe_Q : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AsheTag>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                AsheTag
              , Simulate
              , ActiveItemWithoutState_Request
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        state.Dependency = new Job()
            .Schedule(state.Dependency);
    }

    [WithAll(
        typeof(AsheTag)
      , typeof(Simulate)
      , typeof(ActiveItemWithoutState_Request))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        private void Execute(
            ref DynamicBuffer<IncomingEffectBuffer> incomingEffects
          , ref DynamicBuffer<EffectBuffer>         effectBuffer
          , in  ItemActiveRequestData               request
          , in  ItemSlotsData                       slots
          , ScalerPersonalConstructAspect           personalConstructor
          , in Entity                               entity) {
            if (request.item != SlotItemId.Skill_Q) return;

            incomingEffects.Add(new IncomingEffectBuffer {
                id           = { id = EffectId.AsheSkill_Q_Active, source = entity }
              , senderScaler = personalConstructor.ConstructWithLevel(slots.data.Skill_Q.level)
            });

            // TODO: because this effect not affect to player statistics so this gonna be ok.
            // But in the future, we will need to implement a cleaner method to remove effect (maybe through buffer)
            effectBuffer.Remove(EffectId.AsheSkill_Q_Stack);
        }
    }
}
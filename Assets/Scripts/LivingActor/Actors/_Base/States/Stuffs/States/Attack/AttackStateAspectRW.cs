using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public readonly partial struct AttackStateAspectRW : IAspect {
    private readonly RefRW<AttackStateData> _AttackData;

    [ReadOnly] private readonly DynamicBuffer<StatsBuffer> _Stats;

    public ref readonly AttackStateData Data => ref _AttackData.ValueRO;

    public void RestartAttack(in NetworkTick curTick, int attackSpeedId, int tickRate) {
        _AttackData.ValueRW.cooldownDoneAtTick = curTick
            .WithDeltaTime(1 / _Stats[attackSpeedId].value, tickRate);

        // TODO: Add read attack normalize data of each target (0 -> 1) and apply true real attack tick
        _AttackData.ValueRW.realAttackAtTick = curTick
            .WithDeltaTime(0.2f / _Stats[attackSpeedId].value, tickRate);

        _AttackData.ValueRW.isAttacked = false;
    }
}
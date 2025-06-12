using Unity.Entities;
using Unity.NetCode;

public readonly partial struct AttackStateAspectRW : IAspect {
    private readonly RefRW<AttackStateData> _AttackData;
    private readonly RefRO<StatsData>       _Stats;

    public ref readonly AttackStateData Data => ref _AttackData.ValueRO;

    public void RestartAttack(in NetworkTick curTick, int tickRate) {
        _AttackData.ValueRW.cooldownDoneAtTick = curTick
            .WithDeltaTime(1 / _Stats.ValueRO.data.AttackSpeed, tickRate);

        // TODO: Add read attack normalize data of each target (0 -> 1) and apply true real attack tick
        _AttackData.ValueRW.realAttackAtTick = curTick
            .WithDeltaTime(0.2f / _Stats.ValueRO.data.AttackSpeed, tickRate);

        _AttackData.ValueRW.isAttacked = false;
    }
}
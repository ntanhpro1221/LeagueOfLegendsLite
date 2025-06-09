using Unity.Entities;

public readonly partial struct BountyAspectRW : IAspect {
    [Optional] private readonly EnabledRefRW<BountyTrigger> _Trigger;

    private readonly RefRW<BountyTriggerData> _Data;

    public ref readonly Entity LastHitEntity => ref _Data.ValueRO.lastHitEntity;

    public void TurnOn(in Entity lastHitEntity) {
        _Trigger.ValueRW            = true;
        _Data.ValueRW.lastHitEntity = lastHitEntity;
    }

    public void TurnOff() => _Trigger.ValueRW = false;
}
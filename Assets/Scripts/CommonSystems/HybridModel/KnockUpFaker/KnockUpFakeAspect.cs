using Unity.Entities;
using Unity.NetCode;

public readonly partial struct KnockUpFakeAspect : IAspect {
    private readonly RefRW<KnockUpFakeTriggerData> _Data;

    [Optional] private readonly EnabledRefRW<KnockUpFakeTrigger> _Trigger;

    public void PushKnockUp(in NetworkTick endAtTick) {
        if (!_Trigger.ValueRO
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
         || endAtTick.IsNewerThan(_Data.ValueRO.endAtTick)) {
            _Trigger.ValueRW        = true;
            _Data.ValueRW.endAtTick = endAtTick;
        }
    }
}
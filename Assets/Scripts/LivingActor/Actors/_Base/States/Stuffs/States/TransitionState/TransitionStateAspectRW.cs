using Unity.Entities;
using Unity.NetCode;

public readonly partial struct TransitionStateAspectRW : IAspect {
    private readonly RefRW<TransitionStateData> _TransitionData;
    private readonly RefRW<SharedAnimData>      _AnimData;

    public ref NetworkTick   DoneAtTick  => ref _TransitionData.ValueRW.DoneAtTick;
    public ref SharedAnimKey CurAnim     => ref _AnimData.ValueRW.curAnim;
    public ref bool          HardCutAnim => ref _AnimData.ValueRW.hardCutAnim;
}

public readonly partial struct TransitionStateAspectRO : IAspect {
    private readonly RefRO<TransitionStateData> _TransitionData;
    private readonly RefRO<SharedAnimData>      _AnimData;

    public ref readonly NetworkTick   DoneAtTick  => ref _TransitionData.ValueRO.DoneAtTick;
    public ref readonly SharedAnimKey CurAnim     => ref _AnimData.ValueRO.curAnim;
    public ref readonly bool          HardCutAnim => ref _AnimData.ValueRO.hardCutAnim;
}
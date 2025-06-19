using Unity.Entities;

public readonly partial struct CCAspectRO : IAspect {
    private readonly RefRO<CC.Disable.Receiver> _Disable;
    private readonly RefRO<CC.Control.Receiver> _Control;

    public ref readonly Strum.CC_Disable.Fields<int> Disable => ref _Disable.ValueRO.flags;
    public ref readonly CC.Control.Receiver          Control => ref _Control.ValueRO;
}
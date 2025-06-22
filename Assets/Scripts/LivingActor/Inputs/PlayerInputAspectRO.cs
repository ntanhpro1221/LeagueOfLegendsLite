using Unity.Entities;

public readonly partial struct PlayerInputAspectRO : IAspect {
    private readonly RefRO<PlayerInputData>        _Input;
    private readonly RefRO<PlayerTrigger.PrevCode> _PrevCode;

    public ref readonly PlayerInputData        Input    => ref _Input.ValueRO;
    public ref readonly PlayerTrigger.PrevCode PrevCode => ref _PrevCode.ValueRO;

    public bool MoveEvent_WithData => this.GetEvent_WithData(InputRequestId.Move);
}
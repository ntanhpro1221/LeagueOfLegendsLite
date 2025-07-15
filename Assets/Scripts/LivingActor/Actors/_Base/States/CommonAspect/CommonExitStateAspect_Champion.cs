using Unity.Entities;

public readonly partial struct CommonExitStateAspect_Champion : IAspect {
    private readonly RefRO<ItemActiveRequestData> _ItemRequest;

    public readonly PlayerInputAspectRO Input;

    public ref readonly ItemActiveRequestData ItemRequest => ref _ItemRequest.ValueRO;
}
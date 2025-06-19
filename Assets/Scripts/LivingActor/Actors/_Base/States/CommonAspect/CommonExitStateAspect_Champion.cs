using Unity.Entities;

public readonly partial struct CommonExitStateAspect_Champion : IAspect {
    private readonly RefRO<ItemActiveNewStateRequestData> _ItemRequest;

    public readonly PlayerInputAspectRO Input;

    public ref readonly ItemActiveNewStateRequestData ItemRequest => ref _ItemRequest.ValueRO;
}
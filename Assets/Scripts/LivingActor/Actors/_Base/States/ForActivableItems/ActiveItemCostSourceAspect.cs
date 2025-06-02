using Unity.Entities;

public readonly partial struct ActiveItemCostSourceAspect : IAspect {
    private readonly            RefRW<HealthData> _Health;
    [Optional] private readonly RefRW<ManaData>   _Mana;

    public ref float_Q3 Health => ref _Health.ValueRW.value;
    public ref float_Q3 Mana   => ref _Mana.ValueRW.value;

    public bool IsValid_Mana => _Mana.IsValid;
}
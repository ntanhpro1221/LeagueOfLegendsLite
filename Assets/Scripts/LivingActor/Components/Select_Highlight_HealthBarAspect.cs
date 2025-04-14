using Unity.Entities;

public readonly partial struct Select_Highlight_HealthBarAspect : IAspect {
    #pragma warning disable CS0414 // Field is assigned but its value is never used
    private readonly RefRO<Simulate> _Simulate;
    #pragma warning restore CS0414 // Field is assigned but its value is never used

    [Optional] private readonly EnabledRefRW<HighlightVisible>       _HighlightVisible;
    [Optional] private readonly EnabledRefRW<Selectable>             _Selectable;
    [Optional] private readonly EnabledRefRW<HybridHealthBarVisible> _HealthBarVisible;

    private void SetAll(bool enable)
        => _HighlightVisible.ValueRW = _Selectable.ValueRW = _HealthBarVisible.ValueRW = enable;

    public void EnableAll()  => SetAll(true);
    public void DisableAll() => SetAll(false);
}
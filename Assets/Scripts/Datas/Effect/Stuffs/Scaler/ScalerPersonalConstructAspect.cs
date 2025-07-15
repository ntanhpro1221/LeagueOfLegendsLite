using Unity.Entities;

public readonly partial struct ScalerPersonalConstructAspect : IAspect {
    private readonly RefRO<StatsData>  _Stats;
    private readonly RefRO<HealthData> _Health;

    [Optional] private readonly RefRO<ManaData>  _Mana;
    [Optional] private readonly RefRO<LevelData> _Level;

    public ref readonly Strum.Stats.Fields<float_Q3> Stats => ref _Stats.ValueRO.data;

    public Scaler.Metadata.Personal Construct() => new(
        _Stats.ValueRO
      , _Health.ValueRO
      , _Mana.IsValid ? _Mana.ValueRO : default
      , _Level.IsValid ? _Level.ValueRO : default);

    public Scaler.Metadata.Personal ConstructWithLevel(int level) {
        var result = Construct();
        result.level = level;
        return result;
    }
}
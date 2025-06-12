using Unity.Entities;

public readonly partial struct HealthBarUpdateAspect : IAspect {
    private readonly RefRO<HealthData> _Health;

    private readonly RefRO<StatsData> _Stats;

    [Optional] private readonly RefRO<ManaData>  _Mana;
    [Optional] private readonly RefRO<LevelData> _Level;

    public ref readonly StatsData Stats => ref _Stats.ValueRO;

    public ref readonly LevelData Level => ref _Level.ValueRO;

    public HealthBarUI.UpdateData GenerateUpdateData(in RequireExpData requireExpData) => new() {
        maxHealth              = Stats.data.Health
      , curHealth              = _Health.ValueRO.value
      , curArmor               = 0
      , maxMana                = _Mana.IsValid ? Stats.data.Mana : 0
      , curMana                = _Mana.IsValid ? _Mana.ValueRO.value : 0
      , curLevel               = _Level.IsValid ? _Level.ValueRO.curLevel : 0
      , curExp                 = _Level.IsValid ? _Level.ValueRO.curExp : 0
      , requiredExp            = _Level.IsValid ? requireExpData.CalcRequireExpForNextLevel(_Level.ValueRO.curLevel) : 0
      , ignoreLostHealthEffect = false
    };
}
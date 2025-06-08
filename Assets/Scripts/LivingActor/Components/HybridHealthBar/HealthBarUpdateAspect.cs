using Unity.Collections;
using Unity.Entities;

public readonly partial struct HealthBarUpdateAspect : IAspect {
    private readonly RefRO<HealthData> _Health;

    [ReadOnly] public readonly DynamicBuffer<StatsBuffer> Stats;

    [Optional] private readonly RefRO<ManaData>  _Mana;
    [Optional] private readonly RefRO<LevelData> _Level;

    public bool LevelValid => _Level.IsValid;

    public HealthBarUI.UpdateData GenerateUpdateData(in RequireExpData requireExpData) => new() {
        maxHealth              = Stats[(int)StatsType.Health].value
      , curHealth              = _Health.ValueRO.value
      , curArmor               = 0
      , maxMana                = _Mana.IsValid ? Stats[(int)StatsType.Mana].value : 0
      , curMana                = _Mana.IsValid ? _Mana.ValueRO.value : 0
      , curLevel               = _Level.IsValid ? _Level.ValueRO.curLevel : 0
      , curExp                 = _Level.IsValid ? _Level.ValueRO.curExp : 0
      , requiredExp            = _Level.IsValid ? requireExpData.CalcRequireExpForNextLevel(_Level.ValueRO.curLevel) : 0
      , ignoreLostHealthEffect = false
    };
}
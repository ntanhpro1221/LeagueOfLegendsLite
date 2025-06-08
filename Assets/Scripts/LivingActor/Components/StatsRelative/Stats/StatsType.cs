public enum StatsType {
    Health
  , Mana
  , MoveSpeed
  , Armor
  , SpellBlock
  , AttackRange
  , HealthRegen
  , ManaRegen
  , Crit
  , PhysicDamage
  , MagicDamage
  , AttackSpeed
  , UnitRadius

    // Just for counting
  , _COUNT
}

public static partial class EnumCount {
    public const int Stats = (int)global::StatsType._COUNT;
}

public static class StatsId {
    public const int Health       = (int)StatsType.Health;
    public const int Mana         = (int)StatsType.Mana;
    public const int MoveSpeed    = (int)StatsType.MoveSpeed;
    public const int Armor        = (int)StatsType.Armor;
    public const int SpellBlock   = (int)StatsType.SpellBlock;
    public const int AttackRange  = (int)StatsType.AttackRange;
    public const int HealthRegen  = (int)StatsType.HealthRegen;
    public const int ManaRegen    = (int)StatsType.ManaRegen;
    public const int Crit         = (int)StatsType.Crit;
    public const int PhysicDamage = (int)StatsType.PhysicDamage;
    public const int MagicDamage  = (int)StatsType.MagicDamage;
    public const int AttackSpeed  = (int)StatsType.AttackSpeed;
    public const int UnitRadius   = (int)StatsType.UnitRadius;
}
using System;

[Serializable]
public struct Scaler {
    public Factor   factor;
    public Source   source;
    public StatId   stat;
    public float_Q3 ratio;

    public readonly void Apply(ref float_Q3 origin, in Metadata metadata) =>
        origin += metadata.GetFactor(source, factor, stat) * ratio;

    public enum Factor {
        Stat
      , Health
      , Mana
      , Level
    }

    public enum Source {
        Sender
      , Receiver
    }

    public struct Metadata {
        public Personal sender;
        public Personal receiver;
        public uint     customLifeTick;

        public Metadata(in Personal sender, in Personal receiver, uint customLifeTick) {
            this.sender         = sender;
            this.receiver       = receiver;
            this.customLifeTick = customLifeTick;
        }

        public struct Personal {
            public Strum.Stats.Fields<float_Q3> stats;
            public float_Q3                     health;
            public float_Q3                     mana;
            public int                          level;

            public Personal(
                in StatsData  stats
              , in HealthData health
              , in ManaData   mana
              , in LevelData  level) {
                this.stats  = stats.data;
                this.health = health.value;
                this.mana   = mana.value;
                this.level  = level.curLevel;
            }
        }
    }
}

public static class ScalerExtensions {
    public static ref readonly Scaler.Metadata.Personal GetSource(this in Scaler.Metadata metadata, Scaler.Source source) {
        switch (source) {
            case Scaler.Source.Sender:   return ref metadata.sender;
            case Scaler.Source.Receiver: return ref metadata.receiver;

            default: throw new ArgumentOutOfRangeException(nameof(source), source, $"Found: {source} {(int)source}");
        }
    }

    public static float_Q3 GetFactor(
        this in Scaler.Metadata metadata
      , Scaler.Source           source
      , Scaler.Factor           factor
      , StatId                  stat) {
        ref readonly var personal = ref metadata.GetSource(source);
        return factor switch {
            Scaler.Factor.Stat   => personal.stats[stat]
          , Scaler.Factor.Health => personal.health
          , Scaler.Factor.Mana   => personal.mana
          , Scaler.Factor.Level  => personal.level

          , _ => throw new ArgumentOutOfRangeException(nameof(factor), factor, $"Found: {factor} {(int)factor}")
        };
    }
}
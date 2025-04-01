using System;

[Serializable]
public struct StatBuffData {
    public StatBuffDurationType durationType;
    public float_Q3             duration;
    public ChampionStatsType    targetStat;
    public StatBuffUpdateType   updateType;
    public float_Q3             value;
}
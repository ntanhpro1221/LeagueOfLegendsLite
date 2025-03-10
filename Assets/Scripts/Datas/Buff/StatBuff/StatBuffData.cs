using System;

[Serializable]
public struct StatBuffData {
    public StatBuffDurationType durationType;
    public float                duration;
    public ChampionStatsType    targetStat;
    public StatBuffUpdateType   updateType;
    public float                value;
}
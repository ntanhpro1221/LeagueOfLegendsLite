using Unity.Entities;

public struct BattleClientData : IComponentData {
    public TeamType   teamType;
    public ChampionId champion;

    public static BattleClientData BuildFrom(in TeamMemberBuffer source) => new() {
        teamType = source.team
      , champion = source.champ
    };
}
using System;
using UnityEngine;

public static class ActorAuthoringHelpers {
    public static object ExtractDataFromTag<TAuthoring>(TAuthoring authoring) where TAuthoring : Component {
        var race = authoring.GetComponent<IRaceTag>();
        return race.Race switch {
            RaceId.Champ   => GameSO.Champ[(ChampionId)race.TagInt]
          , RaceId.Minion  => GameSO.Minion[(MinionId)race.TagInt]
          , RaceId.Monster => GameSO.Monster[(MonsterId)race.TagInt]
          , RaceId.Tower   => GameSO.Tower[(TowerId)race.TagInt]
          , _              => throw new Exception($"Cannot get race id, founded: {race.Race} {(int)race.Race} in {authoring.name}")
        };
    }

    public static bool IsBaseRace<TAuthoring>(TAuthoring authoring) where TAuthoring : Component =>
        authoring.GetComponent<IRaceTagAuthoring>() != null;
}
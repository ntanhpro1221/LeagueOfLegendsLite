using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Just exist in server, don't bake it.<br/>
/// This will be created in <see cref="HandleInGameRequest_ServerSystem.OnCreate"/>
/// </summary>
public struct IncomingClientBuffer : IBufferElementData {
    public NetworkId          netId;
    public FixedString32Bytes playerName;
}

public struct TeamMemberBuffer : IBufferElementData, IEquatable<TeamMemberBuffer>, IEquatable<NetworkId> {
    [GhostField] public NetworkId          netId;
    [GhostField] public FixedString32Bytes playerName;

    [GhostField] public TeamType   team;
    [GhostField] public ChampionId champ;
    [GhostField] public bool       lockedChamp;

    public void LockChamp(ChampionId champId) => (lockedChamp, champ) = (true, champId);

    public static TeamMemberBuffer BuildFrom(in IncomingClientBuffer source) => new() {
        netId      = source.netId
      , playerName = source.playerName
      , team       = TeamType.Blue
    };

    public override int GetHashCode() => HashCode.Combine(
        netId.Value
      , (int)team
      , (int)champ
      , playerName.GetHashCode());

    public bool Equals(TeamMemberBuffer other) =>
        netId.Value == other.netId.Value
     && team        == other.team
     && champ       == other.champ
     && playerName  == other.playerName;

    public bool Equals(NetworkId other) =>
        netId.Value == other.Value;

    public override bool Equals(object obj) =>
        obj is TeamMemberBuffer other
     && Equals(other);
}

public struct TeamMemberData : IComponentData {
    [GhostField] public int serverHash;

    public int clientHash;

    public bool NeedUpdateUI => serverHash != clientHash;

    public void UpdateHash_Server(in DynamicBuffer<TeamMemberBuffer> buffer) {
        int newHash = 0;
        foreach (var member in buffer)
            newHash = HashCode.Combine(newHash, member.GetHashCode());
        serverHash = newHash;
    }

    public void UpdateHash_Client() => clientHash = serverHash;
}

public struct BanPickData : IComponentData {
    
}

public class BanPickAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BanPickAuthoring> {
        public override void Bake(BanPickAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddBuffer<TeamMemberBuffer>(entity);
            AddComponent<TeamMemberData>(entity);
            AddComponent<BanPickData>(entity);
        }
    }
}
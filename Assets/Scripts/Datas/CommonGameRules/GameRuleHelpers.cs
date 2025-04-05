using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[BurstCompile]
public static class GameRuleHelpers {
    [BurstCompile]
    public static NetworkTick CalcRespawnTick(
        in DynamicBuffer<BaseRespawnWaitTimeBuffer> BRW
      , in NetworkTime                              networkTime
      , int                                         simulationTickRate
      , int                                         curLevel) {

        NetworkTick respawnTick = networkTime.ServerTick;
        respawnTick.Add((uint)(BRW[curLevel - 1].value * simulationTickRate));
        return respawnTick;
    }
}
using System;
using Unity.NetCode;
using UnityEngine;

public static class TickHelpers {
    /// <param name="deltaTime">in second</param>
    /// <returns></returns>
    public static NetworkTick WithDeltaTime(this NetworkTick curTick, float deltaTime, int tickRate) {
        curTick.Add(CountTick(deltaTime, tickRate));
        return curTick;
    }
    
    public static NetworkTick WithBonusTick(this NetworkTick curTick, uint tick) {
        curTick.Add(tick);
        return curTick;
    }

    public static bool IsNewerThanOrEqual(this NetworkTick lhs, NetworkTick rhs) =>
        lhs.Equals(rhs)
     || lhs.IsNewerThan(rhs);
    
    public static NetworkTick StartTick(float startTime, int tickRate) =>
        new NetworkTick(CountTick(startTime, tickRate));

    public static uint CountTick(float time, int tickRate, RoundMethod method = RoundMethod.Lower) => method switch {
        RoundMethod.Lower   => (uint)(time * tickRate)
      , RoundMethod.Nearest => (uint)Mathf.RoundToInt(time * tickRate)
      , _                   => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };

    public enum RoundMethod {
        Lower   = 0
      , Nearest = 1
    }
}
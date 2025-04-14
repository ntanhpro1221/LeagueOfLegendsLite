using Unity.NetCode;

public static class NetworkTickExtensions {
    /// <param name="deltaTime">in second</param>
    /// <returns></returns>
    public static NetworkTick WithDeltaTime(this NetworkTick curTick, float deltaTime, int tickRate) {
        curTick.Add((uint)(deltaTime * tickRate));
        return curTick;
    }
}
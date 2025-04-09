using Unity.NetCode;

public static class NetworkTickExtensions {
    public static NetworkTick WithDeltaTime(this NetworkTick curTick, float deltaTime, int tickRate) {
        curTick.Add((uint)(deltaTime * tickRate));
        return curTick;
    }
}
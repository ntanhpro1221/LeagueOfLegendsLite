using Unity.NetCode;

public struct TickVersionForInput {
    public NetworkTick MyTick;

    public readonly bool IsValid(in NetworkTick curTick) =>
        curTick.Equals(MyTick);

    public void UpdateTick(in NetworkTick curTick) =>
        MyTick = curTick;
}
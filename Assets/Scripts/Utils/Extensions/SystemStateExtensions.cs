using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public static class SystemStateExtensions {
    public static FixedString32Bytes WorldName(this ref SystemState state)
        => state.WorldUnmanaged.IsClient() ? "Client" : "Server";
}
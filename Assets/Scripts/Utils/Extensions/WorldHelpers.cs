using Unity.Entities;
using Unity.NetCode;

public static class WorldHelpers {
    public static World ClientWorld => ClientServerBootstrap.ClientWorld;
    public static World ServerWorld => ClientServerBootstrap.ServerWorld;

    public static World FindFirstOfType(WorldFlags flag) {
        foreach (var world in World.All)
            if ((world.Flags & flag) != 0)
                return world;

        return null;
    }

    public static void DestroyWorldsOfType(WorldFlags type) {
        while (FindFirstOfType(type) is { } world)
            world.Dispose();
    }

    public static NetworkStreamDriver GetNetworkStreamDriver(this World world)
        => world.EntityManager
            .CreateEntityQuery(typeof(NetworkStreamDriver))
            .GetSingleton<NetworkStreamDriver>();

    public static class Create {
        public static World Client() => ClientServerBootstrap.CreateClientWorld("Client World");

        public static World Server() => ClientServerBootstrap.CreateServerWorld("Server World");

        public static void Host() {
            Server();
            Client();

            World.DefaultGameObjectInjectionWorld = ClientWorld;
        }
    }
}
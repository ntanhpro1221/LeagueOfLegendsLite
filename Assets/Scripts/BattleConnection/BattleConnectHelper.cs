using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;

public static class BattleConnectHelper {
    public static void Connect(BattleConnectData connectData) {
        DestroyLocalSimulationWorld();
        
        switch (connectData.networkRole) {
            case NetworkRole.Client: StartClient(connectData.endpoint); break;
            case NetworkRole.Server: StartServer(connectData.endpoint); break;
            case NetworkRole.Host:   StartHost(connectData.endpoint); break;
            default:                 throw new Exception("Unknown network role");
        }

        World
            .DefaultGameObjectInjectionWorld
            .EntityManager
            .CreateSingleton(connectData.ToBattleInitData());
        
        SceneManager.LoadScene(SceneNameHelper.BattleScene);
    }

    private static void DestroyLocalSimulationWorld() {
        foreach (var world in World.All) {
            if (world.Flags == WorldFlags.Game) {
                world.Dispose();
                break;
            }
        }
    }
    
    private static NetworkStreamDriver GetNetworkStreamDriver(World world) {
        var entityMan                = world.EntityManager;
        var networkStreamDriverQuery = entityMan.CreateEntityQuery(typeof(NetworkStreamDriver));
        var networkStreamDriver      = networkStreamDriverQuery.GetSingleton<NetworkStreamDriver>();
        return networkStreamDriver;
    }

    private static void StartClient(NetworkEndpoint endpoint) {
        var world = ClientServerBootstrap.CreateClientWorld("Client World");
        GetNetworkStreamDriver(world).Connect(world.EntityManager, endpoint);
    }

    private static void StartServer(NetworkEndpoint endpoint) {
        var world = ClientServerBootstrap.CreateServerWorld("Server World");
        GetNetworkStreamDriver(world).Listen(endpoint);
    }

    private static void StartHost(NetworkEndpoint endpoint) {
        StartServer(endpoint);
        StartClient(endpoint);

        World.DefaultGameObjectInjectionWorld = ClientServerBootstrap.ClientWorld;
    }
}
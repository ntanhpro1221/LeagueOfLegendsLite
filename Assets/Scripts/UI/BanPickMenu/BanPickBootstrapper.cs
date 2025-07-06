using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NGDtuanh.Singleton;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Scenes;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BanPickBootstrapper : Singleton<BanPickBootstrapper> {
    public string PlayerName { get; private set; }
    public bool   IsHost     { get; private set; }

    private LanDiscoverer.Server _Server;

    private class SubSceneLoadItem {
        public readonly WorldUnmanaged World;
        public readonly Entity         Entity;

        public SubSceneLoadItem(World world, SubSceneHub.Id sceneId) =>
            Entity = sceneId.LoadAsyncTo(
                World = world.Unmanaged);
    }

    private IEnumerator StartBanPickCoroutine(NetworkRole role, string playerName, string hostIp) {
        var config   = GameSO.RoomConnectionConfig;
        var doServer = role is NetworkRole.Host or NetworkRole.Server;
        var doClient = role is NetworkRole.Host or NetworkRole.Client;

        // Start BanPick scene
        SceneManager.LoadSceneAsync(SceneNameHelper.BanPickScene);

        // Assign value
        PlayerName = playerName;
        IsHost     = role is NetworkRole.Host;

        // Destroy all old worlds
        WorldHelpers.DestroyWorldsOfType(WorldFlags.Game);

        // Load all subscenes
        List<SubSceneLoadItem> subSceneLoads = new();
        if (doServer) {
            var world = WorldHelpers.Create.Server();
            subSceneLoads.Add(new SubSceneLoadItem(world, SubSceneHub.Id.AllGhost));
            subSceneLoads.Add(new SubSceneLoadItem(world, SubSceneHub.Id.BanPick));
        }

        if (doClient) {
            var world = WorldHelpers.Create.Client();
            subSceneLoads.Add(new SubSceneLoadItem(world, SubSceneHub.Id.AllGhost));
            subSceneLoads.Add(new SubSceneLoadItem(world, SubSceneHub.Id.BanPick));
            
            // Reassign default world
            World.DefaultGameObjectInjectionWorld = world;
        }

        // IMPORTANT!!!: Wait for all subscenes loaded
        while (subSceneLoads.Any(item => SceneSystem.IsSceneLoaded(item.World, item.Entity)))
            yield return null;
        
        // Connect and load BanPick subscene
        if (doServer) {
            WorldHelpers.ServerWorld.GetNetworkStreamDriver().Listen(
                NetworkEndpoint.AnyIpv4.WithPort(config.GamePort));

            // Start broadcast this room
            _Server = new LanDiscoverer.Server(
                config.Keyword.RoomBroadcast
              , config.BroadcastPort
              , config.BroadcastSleepTime
              , JsonUtility.ToJson(new RoomBroadcastData { PlayerName = playerName }));
        }

        if (doClient)
            WorldHelpers.ClientWorld.GetNetworkStreamDriver().Connect(
                WorldHelpers.ClientWorld.EntityManager
              , NetworkEndpoint.Parse(hostIp, config.GamePort));
    }

    public void StartBanPick_AsHost(string playerName) => StartCoroutine(StartBanPickCoroutine(
        NetworkRole.Host
      , playerName
      , IPAddress.Loopback.ToString()));

    public void StartBanPick_AsClient(string playerName, string hostIp) => StartCoroutine(StartBanPickCoroutine(
        NetworkRole.Client
      , playerName
      , hostIp));

    public void StopBroadcastRoom() {
        _Server?.Dispose();
        _Server = null;
    }

    private void OnApplicationQuit() {
        StopBroadcastRoom();
    }
}
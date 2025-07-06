using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class SubSceneHub : Singleton<SubSceneHub> {
    public enum Id {
        AllGhost
      , BanPick
      , Battle
    }

    [SerializeField] private EnumMap<Id, SubScene> _SubScenes;

    public SubScene this[Id id] => Instance._SubScenes[id];
}

public static class SubSceneExtensions {
    public static Entity LoadAsyncTo(this SubSceneHub.Id sceneId, in WorldUnmanaged world) =>
        SceneSystem.LoadSceneAsync(world, SubSceneHub.Instance[sceneId].SceneGUID);

    public static Entity LoadAsyncTo(this SubSceneHub.Id sceneId, WorldFlags world) =>
        sceneId.LoadAsyncTo(WorldHelpers.FindFirstOfType(world).Unmanaged);
}
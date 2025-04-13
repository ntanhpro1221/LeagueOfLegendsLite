using NGDtuanh.Singleton;
using UnityEngine;

public class Alice : SceneSingleton<Alice> {
    public GameObject         ashe;
    public AnimatorOverrideController Controller;

    // private void Update() {
    //     // if (Keyboard.current.spaceKey.wasPressedThisFrame) {
    //     //     using var queryBuilder = new EntityQueryBuilder(Allocator.Temp);
    //     //     var query = queryBuilder
    //     //         .WithAll<LocalTransform, ChampionTag>()
    //     //         .Build(World.DefaultGameObjectInjectionWorld.EntityManager);
    //     //     Instantiate(ashe, query.GetSingleton<LocalTransform>().Position, Quaternion.identity);
    //     // }
    // }
    
    public void SpawnAshe(Vector3 position) {
        Instantiate(ashe, position, Quaternion.identity);
    }
}
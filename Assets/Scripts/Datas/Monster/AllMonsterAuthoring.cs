using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllMonsterAuthoring : MonoBehaviour {
    public AllMonsterDataSO MonstersSO;

    public class AllMonsterDataBaker : Baker<AllMonsterAuthoring> {
        public override void Bake(AllMonsterAuthoring authoring) {
            if (authoring.MonstersSO == null) return;
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllMonsterData data = new();
            authoring.MonstersSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}
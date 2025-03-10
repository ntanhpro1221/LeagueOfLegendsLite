using System.Linq;
using BlobAssetExtend;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AllChampionAuthoring : MonoBehaviour {
   public AllChampionDataSO championsSO;
    
    public class AllChampionDataBaker : Baker<AllChampionAuthoring> {
        public override void Bake(AllChampionAuthoring authoring) {
            using var builder = new BlobBuilder(Allocator.Temp);

            ref var hashMap        = ref builder.ConstructRoot<BlobHashMap<EquatableEnum<ChampionId>, ChampionData>>();
            builder.SetHashMap(this, ref hashMap, authoring.championsSO.champions.ToList().ToEquatableEnumCollectionKey());

            var blobRef = builder.CreateBlobAssetReference<BlobHashMap<EquatableEnum<ChampionId>, ChampionData>>(Allocator.Persistent);
            AddBlobAsset(ref blobRef, out var hash);
            
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AllChampionData {
                champions = blobRef
            });
        }
    }
}
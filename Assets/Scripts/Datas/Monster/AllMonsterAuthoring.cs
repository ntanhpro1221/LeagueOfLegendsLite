using System.Linq;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class AllMonsterAuthoring : MonoBehaviour {
    public NetCodeConfig    netConfig;
    public AllMonsterDataSO MonstersSO;

    public class AllMonsterDataBaker : ExtendBaker<AllMonsterAuthoring> {
        public override void Bake(AllMonsterAuthoring authoring) {
            if (authoring.MonstersSO == null
             || authoring.netConfig  == null) return;

            GetDynamicEntity(out var entity);

            AllMonsterData data = new();
            foreach (var sourceItem in authoring.MonstersSO.value.Values)
                sourceItem.staticTickRate = authoring.netConfig.ClientServerTickRate.SimulationTickRate;
            authoring.MonstersSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}
using System.Linq;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class AllMonsterAuthoring : MonoBehaviour {
    public DataSOReader  soReader;
    public NetCodeConfig netConfig;

    public class AllMonsterDataBaker : ExtendBaker<AllMonsterAuthoring> {
        public override void Bake(AllMonsterAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AllMonsterData data = new();
            foreach (var sourceItem in authoring.soReader.Monster.Values)
                sourceItem.staticTickRate = authoring.netConfig.ClientServerTickRate.SimulationTickRate;
            authoring.soReader.Monster.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}
using Unity.Entities;
using UnityEngine;

public struct CommonGameRulesData : IComponentData {
    public int entityRotateSpeed;
}

public class CommonGameRulesAuthoring : MonoBehaviour {
    public int rotateSpeed = 1;

    private class Baker : Baker<CommonGameRulesAuthoring> {
        public override void Bake(CommonGameRulesAuthoring authoring) {
            var eneity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(eneity, new CommonGameRulesData {
                entityRotateSpeed = authoring.rotateSpeed
            });
        }
    }
}
using Unity.Entities;
using UnityEngine;

public struct CommonGameRulesData : IComponentData {
    public int  entityRotateSpeed;
    public int  bountyNearSqr;
    public uint resetAssistTick;
}

public class CommonGameRulesAuthoring : MonoBehaviour {
    public int   rotateSpeed     = 1;
    public float bountyNear      = 1000;
    public float resetAssistTime = 15;

    private class Baker : Baker<CommonGameRulesAuthoring> {
        public override void Bake(CommonGameRulesAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CommonGameRulesData {
                entityRotateSpeed = authoring.rotateSpeed
              , bountyNearSqr     = (int)authoring.bountyNear.Sqr()
              , resetAssistTick = TickHelpers.CountTick(
                    authoring.resetAssistTime
                  , GameSO.TickRate
                  , TickHelpers.RoundMethod.Nearest)
            });
        }
    }
}
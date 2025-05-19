using Unity.Entities;
using UnityEngine;

public struct MinionCommonBehaviourConfigData : IComponentData {
    public float_Q3 aggroCD;
    public float_Q3 reachPathDisToleranceSqr;
}

public class MinionCommonBehaviourConfigAuthoring : MonoBehaviour {
    public float_Q3 aggroCD               = 3;
    public float_Q3 reachPathDisTolerance = 10;

    private class Baker : ExtendBaker<MinionCommonBehaviourConfigAuthoring> {
        public override void Bake(MinionCommonBehaviourConfigAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new MinionCommonBehaviourConfigData {
                aggroCD                  = authoring.aggroCD
              , reachPathDisToleranceSqr = authoring.reachPathDisTolerance.Sqr()
            });
        }
    }
}
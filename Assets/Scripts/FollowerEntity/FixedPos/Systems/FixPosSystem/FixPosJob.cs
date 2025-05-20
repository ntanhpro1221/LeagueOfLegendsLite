using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Pathfinding {
    [WithAll(typeof(FixablePosTrigger))]
    [BurstCompile]
    public partial struct FixPosJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  FixablePosData fixablePos
          , ref LocalTransform locTrans) {
            locTrans.Position.AssignKeepY(fixablePos.pos);
        }
    }
}
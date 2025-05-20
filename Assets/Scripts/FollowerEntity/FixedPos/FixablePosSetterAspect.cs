using Pathfinding.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Pathfinding {
    public readonly partial struct FixablePosSetterAspect : IAspect {
        private readonly RefRW<FixablePosData>                   _FixableData;
        private readonly RefRW<MovementSettings>                 _MovementSettings;
        private readonly RefRW<Pathfinding.ECS.AutoRepathPolicy> _RepathPolicy;
        private readonly RefRW<RVOUpdateData>                    _RVOData;

        [Optional] private readonly EnabledRefRW<FixablePosTrigger> _FixableTrigger;
        [Optional] private readonly EnabledRefRW<RVOUpdateTrigger>  _RVOTrigger;

        private void LockAll(bool value, bool enableAvoidance) {
            if (_FixableTrigger.ValueRO == value
             && _RVOData.ValueRO.enable == enableAvoidance)
                return;

            _RVOTrigger.ValueRW     = true;
            _RVOData.ValueRW.enable = enableAvoidance;

            _FixableTrigger.ValueRW =
                _MovementSettings.ValueRW.isStopped =
                    _RVOData.ValueRW.locked =
                        value;

            _RepathPolicy.ValueRW.mode = value
                ? AutoRepathPolicy.Mode.Never
                : AutoRepathPolicy.Mode.Dynamic;
        }

        /// <summary>
        /// This data does not indicate that this entity is fixed at this pos.<br/>
        /// It depends on <see cref="IsFixedPos"/>.<br/>
        /// </summary>
        public ref readonly float3 FixedPos => ref _FixableData.ValueRO.pos;

        /// <summary>
        /// True when this entity's pos is fixed at <see cref="FixedPos"/>.<br/>
        /// </summary>
        public bool IsFixedPos => _FixableTrigger.ValueRO;

        public void FixAt(float3 pos, bool enableAvoidance = true) {
            LockAll(true, enableAvoidance);
            _FixableData.ValueRW.pos = pos;
        }

        public void Release() => LockAll(false, true);
    }
}
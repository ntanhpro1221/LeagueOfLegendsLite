using Unity.Burst;
using Unity.Entities;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// Convert from <see cref="StateNotExitedYet"/> to <see cref="StateRequireEnter"/>.<br/>
    /// (Run after the <see cref="StateExitSystemGroup"/>)<br/>
    /// (Run before the <see cref="StateEnterSystemGroup"/>)<br/>
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    [UpdateAfter(typeof(StateExitSystemGroup))]
    [UpdateBefore(typeof(StateEnterSystemGroup))]
    public partial struct ConvertExitTag2EnterTagSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (
                    notExitedYet
                  , requireEnter)
                in SystemAPI
                    .Query<
                        EnabledRefRW<StateNotExitedYet>
                      , EnabledRefRW<StateRequireEnter>>()
                    .WithAll<
                        Simulate>()
                    .WithPresent<
                        StateNotExitedYet
                      , StateRequireEnter>()) {
                requireEnter.ValueRW = !notExitedYet.ValueRO;
                notExitedYet.ValueRW = true;
            }
        }
    }
}
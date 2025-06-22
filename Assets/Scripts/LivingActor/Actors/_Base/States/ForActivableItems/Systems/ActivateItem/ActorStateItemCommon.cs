using System;
using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

public static partial class ActorStateItemCommon {
    [UpdateInGroup(typeof(StateExitSystemGroup))]
    public partial struct Exit : ISystem {
        [ReadOnly] private ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();

            selectLookup = SystemAPI.GetComponentLookup<Selectable>(
                isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            selectLookup.Update(ref state);

            var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            foreach (var (
                    filter
                  , common
                  , commonChamp
                  , stateData)
                in SystemAPI.Query<
                    StateFilterAspect
                  , CommonExitStateAspect
                  , CommonExitStateAspect_Champion
                  , RefRW<ItemCommonStateData>>())
                if (SMHelpers.TryExit<ItemCommonState>.ItemCommon(filter, common, commonChamp, stateData.ValueRO, selectLookup, curTick)) {
                    // Do something when exit here
                }
        }

    }

    [UpdateInGroup(typeof(StateEnterSystemGroup))]
    public partial struct Enter : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AllItemData>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var common = new CommonUpdateData {
                allItem = SystemAPI.GetSingleton<AllItemData>()
              , curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick
            };

            foreach (var (_, aspect) in SystemAPI.Query<StateFilterAspect, UpdateAspect>()) {
                common.WithAspect(aspect);

                ref readonly var anim      = ref common.aspect.Anim;
                ref readonly var itemSlots = ref common.aspect.ItemSlots;
                ref var          stateData = ref common.aspect.StateData;
                ref var          rotation  = ref common.aspect.Rotation;

                anim.SetAnim(SharedAnimKey.Idle);

                ref var itemData = ref common.allItem.Items[itemSlots.data[stateData.itemSlot].itemId];

                // Set exit moment
                stateData.performData.Enter(common.curTick, itemData.lifeTick, itemData.settings.triggerPoint);

                // Set follow direction 
                if (itemData.settings.followActivateDir) rotation.RotateTo(stateData.input.direction);

                switch (itemSlots.data[stateData.itemSlot].itemId) {
                    case ItemId.HextechRocketbelt: HextechRocketbelt(ref common); break;

                    default: throw new Exception($"NGDtuanh item active error, this item is invalid, founded: {(int)itemSlots.data[stateData.itemSlot].itemId}");
                }
            }
        }

        private struct CommonUpdateData {
            public AllItemData  allItem;
            public NetworkTick  curTick;
            public UpdateAspect aspect;

            public void WithAspect(in UpdateAspect _aspect) => aspect = _aspect;
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRO<ItemSlotsData>       _ItemSlots;
            private readonly RefRO<LocalTransform>      _LocTrans;
            private readonly RefRW<ItemCommonStateData> _StateData;
            private readonly RefRW<RotationData>        _Rotation;

            public readonly SharedAnimAspect    Anim;
            public readonly MoveRequesterAspect moveRequester;

            public ref readonly ItemSlotsData  ItemSlots => ref _ItemSlots.ValueRO;
            public ref readonly LocalTransform LocTrans  => ref _LocTrans.ValueRO;

            public ref ItemCommonStateData StateData => ref _StateData.ValueRW;
            public ref RotationData        Rotation  => ref _Rotation.ValueRW;
        }
    }

    [UpdateInGroup(typeof(StateUpdateSystemGroup))]
    public partial struct Update : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AllItemData>();
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var netTime = SystemAPI.GetSingleton<NetworkTime>();

            var common = new CommonUpdateData {
                allItem         = SystemAPI.GetSingleton<AllItemData>()
              , ecb             = new EntityCommandBuffer(Allocator.Temp)
              , curTick         = netTime.ServerTick
              , isFirstTimeFull = netTime.IsFirstTimeFullyPredictingTick
            };

            foreach (var (_, data) in SystemAPI.Query<StateFilterAspect, UpdateAspect>()) {
                common.WithAspect(data);
                
                switch (data.ItemSlots.Slots[data.StateData.itemSlot].itemId) {
                    case ItemId.HextechRocketbelt: HextechRocketbelt(ref common, ref state); break;

                    default: throw new Exception($"NGDtuanh item active error, this item is invalid, founded: {(int)data.ItemSlots.Slots[data.StateData.itemSlot].itemId}");
                }
            }

            common.CompleteECB(ref state);
        }

        private struct CommonUpdateData {
            public AllItemData         allItem;
            public EntityCommandBuffer ecb;
            public NetworkTick         curTick;
            public bool                isFirstTimeFull;
            public UpdateAspect        aspect;

            public void WithAspect(in UpdateAspect _aspect) => aspect = _aspect;

            public void CompleteECB(ref SystemState state) {
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
        }

        private readonly partial struct UpdateAspect : IAspect {
            private readonly RefRW<ItemCommonStateData>  _StateData;
            private readonly RefRO<LocalTransform>       _LocTrans;
            private readonly RefRO<ProjectileSpawnPoint> _ProjectileSpawnPoint;
            private readonly RefRO<TeamTypeData>         _TeamData;

            public readonly Entity                        Entity;
            public readonly ItemSlotsAspectRO             ItemSlots;
            public readonly ScalerPersonalConstructAspect PersonalConstructor;
            public readonly MoveRequesterAspect           MoveRequester;

            public ref ItemCommonStateData StateData => ref _StateData.ValueRW;

            public ref readonly float3        Position             => ref _LocTrans.ValueRO.Position;
            public ref readonly InitTransform ProjectileSpawnPoint => ref _ProjectileSpawnPoint.ValueRO.point;
            public ref readonly TeamTypeData  Team                 => ref _TeamData.ValueRO;
        }
    }
}

public static partial class ActorStateItemCommon {
    public partial struct Exit {
        private readonly partial struct StateFilterAspect : IAspect, IStateExitAspect<Simulate, ItemCommonState> {
            private readonly RefRO<Simulate>                 _simulate;
            private readonly EnabledRefRW<StateNotExitedYet> _stateNotExitedYet;
            private readonly EnabledRefRW<ItemCommonState>   _curStateEnable;

            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.Identity => _simulate;
            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.Simulate => _simulate;

            EnabledRefRW<StateNotExitedYet> IStateExitFunc<ItemCommonState>.StateNotExitedYet => _stateNotExitedYet;
            EnabledRefRW<ItemCommonState> IStateExitFunc<ItemCommonState>.  CurStateEnable    => _curStateEnable;
        }
    }

    public partial struct Enter {
        private readonly partial struct StateFilterAspect : IAspect, IStateEnterAspect<Simulate, ItemCommonState> {
            private readonly RefRO<ItemCommonState> _curState;
            private readonly RefRO<Simulate>        _simulate;

            private readonly EnabledRefRO<StateRequireEnter> _stateRequireEnter;

            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Identity => _simulate;
            RefRO<ItemCommonState> IStateAspect<Simulate, ItemCommonState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Simulate => _simulate;

            EnabledRefRO<StateRequireEnter> IStateEnterAspect<Simulate, ItemCommonState>.StateRequireEnter => _stateRequireEnter;
        }
    }

    public partial struct Update {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<Simulate, ItemCommonState> {
            private readonly RefRO<ItemCommonState> _curState;
            private readonly RefRO<Simulate>        _simulate;

            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Identity => _simulate;
            RefRO<ItemCommonState> IStateAspect<Simulate, ItemCommonState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Simulate => _simulate;
        }
    }

    public partial struct FixedUpdate {
        private readonly partial struct StateFilterAspect : IAspect, IStateAspect<Simulate, ItemCommonState> {
            private readonly RefRO<ItemCommonState> _curState;
            private readonly RefRO<Simulate>        _simulate;

            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Identity => _simulate;
            RefRO<ItemCommonState> IStateAspect<Simulate, ItemCommonState>.CurState => _curState;
            RefRO<Simulate> IStateAspect<Simulate, ItemCommonState>.       Simulate => _simulate;
        }
    }
}
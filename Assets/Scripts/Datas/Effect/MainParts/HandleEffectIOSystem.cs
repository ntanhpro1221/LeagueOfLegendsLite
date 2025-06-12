using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleEffectSystemGroup), OrderFirst = true)]
public partial struct HandleEffectIOSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<AllEffectData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick
          , allEffects = SystemAPI.GetSingleton<AllEffectData>()
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick   curTick;
        public AllEffectData allEffects;

        [BurstCompile]
        public void Execute(
            ref DynamicBuffer<EffectBuffer>         effects
          , ref EffectBufferHashData                effectHash
          , ref DynamicBuffer<IncomingEffectBuffer> incomingEffects
          , ref CC.Disable.Receiver                 ccDisableReceiver
          , ref CC.Control.Receiver                 ccControlReceiver
          , ref StatBuffs.Receiver                  statBuffReceiver
          , in  LocalTransform                      locTrans
          , ScalerPersonalConstructAspect           personalConstructor) {
            var receiverData = personalConstructor.Construct();

            // ADD INCOMING EFFECTS
            foreach (var incomingEffect in incomingEffects) {
                ref var newEffectRaw = ref allEffects.Effects[incomingEffect.id.id];

                var finalData = new EffectBuffer(ref newEffectRaw, incomingEffect, receiverData, curTick);

                // Handle CC control effect separately from the other effect parts
                ccControlReceiver.TryAdd(
                    newEffectRaw.ccControl
                  , incomingEffect.id
                  , finalData.endAtTick
                  , (locTrans.Position.Quantizate3() - incomingEffect.senderPos).xz);

                bool createNewInstance = true;

                if (finalData.stackingBehaviour.createNewInstance) {
                    // Since we can create new instance => this is multi-source effect.
                    // Even so, we will create a new instance only if there isn't any effect that has a fully equal ID (include its source) to this new effect.
                    // Otherwise, just do nothing at this moment.
                    for (int i = 0; i < effects.Length; ++i)
                        if (effects[i].id.Equals(finalData.id)) {
                            createNewInstance = false;
                            break;
                        }
                } else {
                    // We cannot create a new instance if the effect with the same id has existed.
                    // In that case, just stack them together.
                    for (int i = 0; i < effects.Length; ++i)
                        if (effects[i].id.id == finalData.id.id) {
                            effects.ElementAt(i).StackWith(finalData, curTick, ref ccDisableReceiver, ref statBuffReceiver);

                            createNewInstance = false;
                            break;
                        }
                }

                if (createNewInstance) {
                    effects.Add(finalData);
                    finalData.AddToReceivers(ref ccDisableReceiver, ref statBuffReceiver);
                }
            }

            // REMOVE EXPIRED EFFECTS
            for (int i = 0; i < effects.Length;) {
                ref var effect = ref effects.ElementAt(i);

                // Unstack and update effect datas
                while (effect.curStack > 0
                 && curTick.IsNewerThan(effect.endAtTick))
                    effect.Unstack(ref ccDisableReceiver, ref statBuffReceiver);

                // Remove effect if there is no stack left
                if (effect.curStack > 0) ++i;
                else effects.RemoveAt(i);
            }

            // DEACTIVATE EXPIRED CC CONTROL
            if (ccControlReceiver.IsActive
             && curTick.IsNewerThan(ccControlReceiver.endAtTick))
                ccControlReceiver.Deactivate();

            // CLEAR INCOMING BUFFER
            incomingEffects.Clear();

            // UPDATE EFFECT HASH
            effectHash.serverHash = 0;
            foreach (var effect in effects)
                effectHash.serverHash = HashCode.Combine(
                    effectHash.serverHash
                  , effect.id.GetHashCode());
        }
    }
}
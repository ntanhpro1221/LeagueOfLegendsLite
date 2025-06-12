using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct EffectFullId : IEquatable<EffectFullId> {
    public EffectId id;
    public Entity   source;

    public bool Equals(EffectFullId other) =>
        id == other.id
     && source.Equals(other.source);

    public override bool Equals(object obj) =>
        obj is EffectFullId other
     && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        (int)id
      , source.GetHashCode());
}

public struct EffectBuffer : IBufferElementData {
    [GhostField] public EffectFullId id;
    [GhostField] public NetworkTick  endAtTick;
    [GhostField] public uint         stackTick;
    [GhostField] public int          curStack;

    [GhostField] public EffectStackingBehaviour stackingBehaviour;

    [GhostField] public CC.Disable.Final     ccDisable;
    [GhostField] public DamageOverTime.Final damageOT;
    [GhostField] public StatBuffs.Final      statBuffs;

    public EffectBuffer(
        ref EffectData               rawData
      , in  IncomingEffectBuffer     incomingEffect
      , in  Scaler.Metadata.Personal receiverData
      , in  NetworkTick              curTick) {
        var metadata = new Scaler.Metadata(
            sender: incomingEffect.senderScaler
          , receiver: receiverData
          , customLifeTick: incomingEffect.customLifeTick);

        id = incomingEffect.id;
        endAtTick = curTick.WithBonusTick(
            stackTick = rawData.fixedLife.GetLifeTick(metadata));
        curStack = 1;

        stackingBehaviour = rawData.stackingBehaviour;

        ccDisable = rawData.ccDisable.ComputeFinal(metadata);
        damageOT  = rawData.damageOT.ComputeFinal(metadata);
        statBuffs = rawData.statBuffs.ComputeFinal(metadata);
    }

    public readonly void AddToReceivers(
        ref CC.Disable.Receiver ccDisableReceiver
      , ref StatBuffs.Receiver  statBuffReceiver) {
        ccDisableReceiver.Add(ccDisable);
        statBuffReceiver.Add(statBuffs);
    }

    public readonly void RemoveFromReceivers(
        ref CC.Disable.Receiver ccDisableReceiver
      , ref StatBuffs.Receiver  statBuffReceiver) {
        ccDisableReceiver.Remove(ccDisable);
        statBuffReceiver.Remove(statBuffs);
    }

    public void StackWith(
        in  EffectBuffer        target
      , in  NetworkTick         curTick
      , ref CC.Disable.Receiver ccDisableReceiver
      , ref StatBuffs.Receiver  statBuffReceiver) {
        // Merge time
        if (stackingBehaviour.resetTimer)
            endAtTick = curTick.WithBonusTick(
                stackTick = math.max(stackTick, target.stackTick));

        // Increase stack count
        if (stackingBehaviour.increaseStackCount
         && stackingBehaviour.maxStack > curStack)
            ++curStack;

        // Update effect powers that depend on stack
        if (stackingBehaviour.stackAffectPower) {
            RemoveFromReceivers(ref ccDisableReceiver, ref statBuffReceiver);

            ccDisable.StackWith(target.ccDisable);
            damageOT.StackWith(target.damageOT);
            statBuffs.StackWith(target.statBuffs);

            AddToReceivers(ref ccDisableReceiver, ref statBuffReceiver);
        }
    }

    public void Unstack(
        ref CC.Disable.Receiver ccDisableReceiver
      , ref StatBuffs.Receiver  statBuffReceiver) {
        // Update timer
        if (stackingBehaviour.useStackForLifeTime)
            endAtTick = endAtTick.WithBonusTick(stackTick);

        // Update stack count
        int oldStack = curStack;
        if (stackingBehaviour.useStackForLifeTime)
            --curStack;
        else curStack = 0;

        // Update effect powers that depend on stack
        if (stackingBehaviour.stackAffectPower
            // When curStack is zero (effect disappear), we will update even when stack does not affect power.
            // Because in this case, this is removing effect, not stacking effect behaviour.
         || curStack == 0) {
            RemoveFromReceivers(ref ccDisableReceiver, ref statBuffReceiver);

            ccDisable.Unstack(oldStack, curStack);
            damageOT.Unstack(oldStack, curStack);
            statBuffs.Unstack(oldStack, curStack);

            AddToReceivers(ref ccDisableReceiver, ref statBuffReceiver);
        }
    }
}

public struct IncomingEffectBuffer : IBufferElementData {
    [GhostField] public EffectFullId             id;
    [GhostField] public Scaler.Metadata.Personal senderScaler;
    [GhostField] public uint                     customLifeTick;
    [GhostField] public float3_Q3                senderPos;
}

public struct EffectBufferHashData : IComponentData {
    [GhostField] public int serverHash;

    public int clientHash;

    public readonly bool NeedFix => serverHash != clientHash;
}

public class EffectAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<EffectAuthoring> {
        public override void Bake(EffectAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddBuffer<EffectBuffer>(entity);
            AddBuffer<IncomingEffectBuffer>(entity);
            AddComponent<EffectBufferHashData>(entity);

            AddComponent<CC.Disable.Receiver>(entity);
            AddComponent<CC.Control.Receiver>(entity);
            AddComponent<StatBuffs.Receiver>(entity);
        }
    }
}
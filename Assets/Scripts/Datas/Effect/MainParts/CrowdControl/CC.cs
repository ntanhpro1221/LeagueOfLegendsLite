using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public static class CC {
    /// <summary>
    /// CC of this type can be stacked together.
    /// </summary>
    public struct Disable : IBlobBuildable<Disable.Managed>, IBlobBuildableSelf<Disable> {
        public bool enable;

        public Strum.CC_Disable.Fields<bool> flags;

        public void BuildBlob(ref BlobBuilder builder, Managed source) {
            enable = source.enable;
            flags  = source.flags;
        }

        public void BuildBlob(ref BlobBuilder builder, ref Disable source) {
            enable = source.enable;
            flags  = source.flags;
        }

        public Final ComputeFinal(in Scaler.Metadata metadata) {
            if (!enable) return default;
            
            return new Final {
                enable = enable
              , flags  = flags
            };
        }

        public struct Final {
            public bool enable;

            public Strum.CC_Disable.Fields<bool> flags;

            public void StackWith(in Final target) {
                if (!enable) return;

                // Nothing to do
            }

            public void Unstack(int oldStack, int newStack) {
                if (!enable) return;
                
                if (newStack > 0) return;

                this = default;
            }
        }
        
        public struct Receiver : IComponentData {
            [GhostField] public Strum.CC_Disable.Fields<int> flags;

            public void Add(in Final final) {
                if (!final.enable) return;
                
                foreach (var index in Strum.CC_Disable.Indexes)
                    if (final.flags[index])
                        ++flags[index];
            }
            
            public void Remove(in Final final) {
                if (!final.enable) return;
                
                foreach (var index in Strum.CC_Disable.Indexes)
                    if (final.flags[index])
                        --flags[index];
            }
        }

        [Serializable]
        public class Managed : IEnableable {
            [field: SerializeField] public override bool enable { get; set; }

            public Strum.CC_Disable.Fields<bool> flags;
        }
    }

    /// <summary>
    /// There can be at most one CC of this type at a time.<br/>
    /// When merging two CCs of this type, the one that ends later will be kept.<br/>
    /// </summary>
    public struct Control : IBlobBuildable<Control.Managed>, IBlobBuildableSelf<Control> {
        public bool enable;

        public CC_ControlId flag;

        public void BuildBlob(ref BlobBuilder builder, Managed source) {
            enable = source.enable;
            flag   = source.flag;
        }

        public void BuildBlob(ref BlobBuilder builder, ref Control source) {
            enable = source.enable;
            flag   = source.flag;
        }

        public struct Receiver : IComponentData {
            [GhostField] public EffectFullId id;
            [GhostField] public NetworkTick  endAtTick;

            [GhostField] public CC_ControlId flag;
            
            [GhostField] public floatXZ_Q3 runAwayDir;

            public readonly bool IsActive => id.source != Entity.Null;

            public void Deactivate() => id.source = Entity.Null;

            public void TryAdd(
                in Control      final
              , in EffectFullId _id
              , in NetworkTick  _endAtTick
              , in floatXZ_Q3   _runAwayDir) {
                if (!final.enable) return;

                if (IsActive && endAtTick.IsNewerThan(_endAtTick)) return;

                id        = _id;
                endAtTick = _endAtTick;
                flag      = final.flag;
                runAwayDir = _runAwayDir;
            }
        }

        [Serializable]
        public class Managed : IEnableable {
            [field: SerializeField] public override bool enable { get; set; }

            public CC_ControlId flag;
        }
    }
}
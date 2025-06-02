using System;
using System.Linq;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct SharedAnimData : IComponentData {
    [GhostField] public SharedAnimKey curAnim;

    [GhostField] public int currentSessionToRestart;

    [GhostField] public bool hardCutAnim;

    public     BlobAssetReference<BubleEnMap<SharedAnimKey, uint>> _AnimLengthTicksRef;
    public ref BubleEnMap<SharedAnimKey, uint>                     AnimLengthTicks => ref _AnimLengthTicksRef.Value;

    public void MarkNeedRestart() => ++currentSessionToRestart;
}

public class SharedAnimAuthoring : MonoBehaviour {
    public SharedAnimKey             entryAnim;
    public bool                      hardCutAnim;
    public string                    animClipPrefix;
    public RuntimeAnimatorController animController;

    private class Baker : ExtendBaker<SharedAnimAuthoring> {
        public override void Bake(SharedAnimAuthoring authoring) {
            var data = new SharedAnimData();

            data.curAnim     = authoring.entryAnim;
            data.hardCutAnim = authoring.hardCutAnim;
            GetAnimLengthTicks(
                    authoring.animClipPrefix
                  , authoring.animController)
                .CreateBlobAssetReferenceInBaker(out data._AnimLengthTicksRef, this, out _);

            AddComponent(GetDynamicEntity(), data);
        }

        private CovEnumMap<SharedAnimKey, uint> GetAnimLengthTicks(
            string                    prefix
          , RuntimeAnimatorController animController) {

            CovEnumMap<SharedAnimKey, uint> result = new();

            var clips    = animController.animationClips;
            int tickRate = GameSO.TickRate;
            
            foreach (SharedAnimKey key in Enum.GetValues(typeof(SharedAnimKey))) {
                string trueName = prefix                                 + key.KeyName();
                string baseName = SharedAnimKeyExtensions.BaseClipPrefix + key.KeyName();

                var clip = clips.FirstOrDefault(clip =>
                    clip.name == trueName
                 || clip.name == baseName);

                if (clip == null)
                    throw new Exception($"Can't build anim lengths. Can't find clip with name '{trueName}' or '{baseName}'");

                result[key] = TickHelpers.CountTick(clip.length, tickRate, TickHelpers.RoundMethod.Nearest);
            }

            return result;
        }
    }
}
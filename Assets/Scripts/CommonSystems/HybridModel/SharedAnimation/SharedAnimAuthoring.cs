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

    public     BlobAssetReference<BubleEnMap<SharedAnimKey, float>> _AnimLengthsRef;
    public ref BubleEnMap<SharedAnimKey, float>                     AnimLengths => ref _AnimLengthsRef.Value;

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
            GetAnimLengths(
                    authoring.animClipPrefix
                  , authoring.animController)
                .CreateBlobAssetReferenceInBaker(out data._AnimLengthsRef, this, out _);

            AddComponent(GetDynamicEntity(), data);
        }

        private CovEnumMap<SharedAnimKey, float> GetAnimLengths(
            string                    prefix
          , RuntimeAnimatorController animController) {

            CovEnumMap<SharedAnimKey, float> result = new();

            var clips = animController.animationClips;

            foreach (SharedAnimKey key in Enum.GetValues(typeof(SharedAnimKey))) {
                string trueName = prefix                                 + key.KeyName();
                string baseName = SharedAnimKeyExtensions.BaseClipPrefix + key.KeyName();

                var clip = clips.FirstOrDefault(clip =>
                    clip.name == trueName
                 || clip.name == baseName);

                if (clip == null)
                    throw new Exception($"Can't build anim lengths. Can't find clip with name '{trueName}' or '{baseName}'");

                result[key] = clip.length;
            }

            return result;
        }
    }
}
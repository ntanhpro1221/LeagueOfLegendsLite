using System;
using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;

public struct ScalableFloat : IBlobBuildable<ScalableFloat.Managed>, IBlobBuildableSelf<ScalableFloat> {
    public float_Q3           origin;
    public BubleArray<Scaler> scalers;

    public readonly float_Q3 GetScaledValue(in Scaler.Metadata metadata) {
        float_Q3 result = origin;
        for (int i = 0; i < scalers.Count; ++i)
            scalers[i].Apply(ref result, metadata);
        return result;
    }

    public void BuildBlob(ref BlobBuilder builder, Managed source) {
        origin = source.origin;
        scalers.BuildBlob(ref builder, source.scalers);
    }

    public void BuildBlob(ref BlobBuilder builder, ref ScalableFloat source) {
        origin = source.origin;
        scalers.BuildBlob(ref builder, ref source.scalers);
    }

    [Serializable]
    public class Managed {
        public float_Q3     origin;
        public List<Scaler> scalers;
    }
}
using System;
using UnityEngine;

[Serializable]
public struct MinMaxCurve3 {
    public ParticleSystem.MinMaxCurve x, y, z;

    public Vector3 Evaluate(float time, float lerpFactor) => new() {
        x = x.Evaluate(time, lerpFactor)
      , y = y.Evaluate(time, lerpFactor)
      , z = z.Evaluate(time, lerpFactor)
    };
}
using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct float3_Q3 : IEquatable<float3_Q3> {
    public const           int       MULTIPLIER = 1000;
    public static readonly float3_Q3 zero       = new(0, 0, 0);
    public static readonly float3_Q3 up         = new(0, 0, MULTIPLIER);
    public static readonly float3_Q3 down       = new(0, 0, -MULTIPLIER);
    public static readonly float3_Q3 forward    = new(0, MULTIPLIER, 0);
    public static readonly float3_Q3 back       = new(0, -MULTIPLIER, 0);
    public static readonly float3_Q3 right      = new(MULTIPLIER, 0, 0);
    public static readonly float3_Q3 left       = new(-MULTIPLIER, 0, 0);

    public int x;
    public int y;
    public int z;

    public float3_Q3(float x, float y, float z) {
        this.x = (int)(x * MULTIPLIER);
        this.y = (int)(y * MULTIPLIER);
        this.z = (int)(z * MULTIPLIER);
    }

    public float3_Q3(float xyz) : this(xyz, xyz, xyz) { }
    
    public float3_Q3(int x, int y, int z) {
        this.x = (x * MULTIPLIER);
        this.y = (y * MULTIPLIER);
        this.z = (z * MULTIPLIER);
    }

    public float3_Q3(int xyz) : this(xyz, xyz, xyz) { }

    public bool Equals(float3_Q3 other) =>
        x == other.x
     && y == other.y
     && z == other.z;

    #region CAST

    public static explicit operator float3_Q3(float3 source) => new(
        source.x
      , source.y
      , source.z);

    public static implicit operator float3(float3_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER);

    public static explicit operator float3_Q3(Vector3 source) => new(
        source.x
      , source.y
      , source.z);

    public static implicit operator Vector3(float3_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER);

    #endregion

    #region OPERATOR

    public static float3_Q3 operator +(float3_Q3 a, float3_Q3 b) => new() {
        x = a.x + b.x
      , y = a.y + b.y
      , z = a.z + b.z
    };

    public static float3_Q3 operator -(float3_Q3 a, float3_Q3 b) => new() {
        x = a.x - b.x
      , y = a.y - b.y
      , z = a.z - b.z
    };

    public static float3_Q3 operator *(float3_Q3 a, int mul) => new() {
        x = a.x * mul
      , y = a.y * mul
      , z = a.z * mul
    };

    public static float3_Q3 operator *(float3_Q3 a, float mul) => new() {
        x = (int)(a.x * mul)
      , y = (int)(a.y * mul)
      , z = (int)(a.z * mul)
    };

    public static float3_Q3 operator /(float3_Q3 a, int div) => new() {
        x = a.x / div
      , y = a.y / div
      , z = a.z / div
    };

    public static float3_Q3 operator /(float3_Q3 a, float div) => new() {
        x = (int)(a.x / div)
      , y = (int)(a.y / div)
      , z = (int)(a.z / div)
    };

    #endregion
}
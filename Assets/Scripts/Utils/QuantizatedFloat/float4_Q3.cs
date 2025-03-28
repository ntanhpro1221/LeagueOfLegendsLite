using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct float4_Q3 : IEquatable<float4_Q3> {
    public const           int       MULTIPLIER = 1000;
    public static readonly float4_Q3 zero       = new(0, 0, 0, 0);
    public static readonly float4_Q3 identity   = new(0, 0, 0, MULTIPLIER);
    
    public int x;
    public int y;
    public int z;
    public int w;

    public float4_Q3(float x, float y, float z, float w) {
        this.x = (int)(x * MULTIPLIER);
        this.y = (int)(y * MULTIPLIER);
        this.z = (int)(z * MULTIPLIER);
        this.w = (int)(w * MULTIPLIER);
    }

    public float4_Q3(float xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }
    
    public float4_Q3(int x, int y, int z, int w) {
        this.x = x * MULTIPLIER;
        this.y = y * MULTIPLIER;
        this.z = z * MULTIPLIER;
        this.w = w * MULTIPLIER;
    }

    public float4_Q3(int xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

    public bool Equals(float4_Q3 other) =>
        x == other.x
     && y == other.y
     && z == other.z
     && w == other.w;

    #region CAST

    public static explicit operator float4_Q3(float4 source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator float4(float4_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER
      , (float)source.w / MULTIPLIER);

    public static explicit operator float4_Q3(Vector4 source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator Vector4(float4_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER
      , (float)source.w / MULTIPLIER);

    public static explicit operator float4_Q3(quaternion source) => new(
        source.value.x
      , source.value.y
      , source.value.z
      , source.value.w);

    public static implicit operator quaternion(float4_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER
      , (float)source.w / MULTIPLIER);

    public static explicit operator float4_Q3(Quaternion source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator Quaternion(float4_Q3 source) => new(
        (float)source.x / MULTIPLIER
      , (float)source.y / MULTIPLIER
      , (float)source.z / MULTIPLIER
      , (float)source.w / MULTIPLIER);

    #endregion

    #region OPERATOR

    public static float4_Q3 operator +(float4_Q3 a, float4_Q3 b) => new() {
        x = a.x + b.x
      , y = a.y + b.y
      , z = a.z + b.z
      , w = a.w + b.w
    };

    public static float4_Q3 operator -(float4_Q3 a, float4_Q3 b) => new() {
        x = a.x - b.x
      , y = a.y - b.y
      , z = a.z - b.z
      , w = a.w - b.w
    };

    public static float4_Q3 operator *(float4_Q3 a, int mul) => new() {
        x = a.x * mul
      , y = a.y * mul
      , z = a.z * mul
      , w = a.w * mul
    };

    public static float4_Q3 operator *(float4_Q3 a, float mul) => new() {
        x = (int)(a.x * mul)
      , y = (int)(a.y * mul)
      , z = (int)(a.z * mul)
      , w = (int)(a.w * mul)
    };

    public static float4_Q3 operator /(float4_Q3 a, int div) => new() {
        x = a.x / div
      , y = a.y / div
      , z = a.z / div
      , w = a.w / div
    };

    public static float4_Q3 operator /(float4_Q3 a, float div) => new() {
        x = (int)(a.x / div)
      , y = (int)(a.y / div)
      , z = (int)(a.z / div)
      , w = (int)(a.w / div)
    };

    #endregion
}
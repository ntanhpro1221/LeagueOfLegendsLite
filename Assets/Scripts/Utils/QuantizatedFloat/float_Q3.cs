using System;

[Serializable]
public struct float_Q3 : IEquatable<float_Q3> {
    public const int Multiplier = 1000;
    public const float Epsilon = 0.001f;

    public int value;

    public float_Q3(float value) {
        this.value = (int)(value * Multiplier);
    }
    
    public float_Q3(int value) {
        this.value = value * Multiplier;
    }

    public bool Equals(float_Q3 other) =>
        value == other.value;

    #region CAST
    
    public static explicit operator float_Q3(float source) =>
        new(source);

    public static implicit operator float(float_Q3 source) =>
        (float)source.value / Multiplier;
    
    public static implicit operator float_Q3(int source) =>
        new(source);

    public static explicit operator int(float_Q3 source) =>
        source.value / Multiplier;
    
    #endregion
    
    #region OPERATOR

    public static float_Q3 operator +(float_Q3 a, float_Q3 b) => new() {
        value = a.value + b.value
    };

    public static float_Q3 operator -(float_Q3 a, float_Q3 b) => new() {
        value = a.value - b.value
    };

    public static float_Q3 operator *(float_Q3 a, int mul) => new() {
        value = a.value * mul
    };

    public static float_Q3 operator *(float_Q3 a, float mul) => new() {
        value = (int)(a.value * mul)
    };

    public static float_Q3 operator /(float_Q3 a, int div) => new() {
        value = a.value / div
    };

    public static float_Q3 operator /(float_Q3 a, float div) => new() {
        value = (int)(a.value / div)
    };
    
    #endregion
}
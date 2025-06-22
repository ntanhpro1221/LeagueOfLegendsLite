using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct float_Q3 : IEquatable<float_Q3>, IFormattable {
    public const float MULTIPLIER = 1000;
    public const float EPSILON    = 10f / MULTIPLIER;

    public int value;

    public float_Q3(float value) {
        this.value = Mathf.RoundToInt(value * MULTIPLIER);
    }

    public float_Q3(int value) {
        this.value = value * (int)MULTIPLIER;
    }

    public override string ToString() => ((float)this).ToString(CultureInfo.CurrentCulture);

    public string ToString(string format, IFormatProvider formatProvider) => format switch {
        "percent"  => $"{(int)(this * 100)}%"
      , "int"      => $"{(int)this}"
      , "float2"   => $"{(float)this:F2}"
      , null or "" => ToString()

      , _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"Float_Q3 format error: Founded: '{format}'")
    };

#region CAST

    public static explicit operator float_Q3(float source) =>
        new(source);

    public static implicit operator float(float_Q3 source) =>
        source.value / MULTIPLIER;

    public static implicit operator float_Q3(int source) =>
        new(source);

    public static explicit operator int(float_Q3 source) =>
        Mathf.RoundToInt(source.value / MULTIPLIER);

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

    public static float_Q3 operator -(float_Q3 source) => new() {
        value = -source.value
    };

    public static float_Q3 operator *(float_Q3 a, float mul) => new() {
        value = Mathf.RoundToInt(a.value * mul)
    };

    public static float_Q3 operator /(float_Q3 a, int div) => new() {
        value = Mathf.RoundToInt(a.value / (float)div)
    };

    public static float_Q3 operator /(float_Q3 a, float div) => new() {
        value = Mathf.RoundToInt(a.value / div)
    };
    
    public static bool operator <(float_Q3 a, float_Q3 b) => a.value < b.value;
    
    public static bool operator >(float_Q3 a, float_Q3 b) => a.value > b.value;
    
    public static bool operator <=(float_Q3 a, float_Q3 b) => a.value <= b.value;
    
    public static bool operator >=(float_Q3 a, float_Q3 b) => a.value >= b.value;
    
    public static bool operator ==(float_Q3 a, float_Q3 b) => a.value == b.value;
    
    public static bool operator !=(float_Q3 a, float_Q3 b) => a.value != b.value;
    
    public override bool Equals(object obj) => obj is float_Q3 other && Equals(other);
    
    public override int GetHashCode() => value.GetHashCode();

    public bool Equals(float_Q3 other) => value == other.value;

#endregion

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(float_Q3))]
    private class float_Q3Drawer : PropertyDrawer {
        private Dictionary<string, SerializedProperty> Properties { get; } = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string path = property.propertyPath;
            if (!Properties.ContainsKey(path)) Properties.Add(path, property.FindPropertyRelative(nameof(value)));
            var drawer = Properties[path];

            position.height = EditorGUIUtility.singleLineHeight;
            drawer.intValue = Mathf.RoundToInt(
                MULTIPLIER
              * EditorGUI.FloatField(
                    position
                  , label
                  , drawer.intValue / MULTIPLIER));
        }
    }

    #endif
}
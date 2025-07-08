using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct float4_Q3 : IEquatable<float4_Q3> {
    public const           float     MULTIPLIER = 1000;
    public static readonly float4_Q3 zero       = new(0, 0, 0, 0);
    public static readonly float4_Q3 identity   = new(0, 0, 0, 1);

    public int x;
    public int y;
    public int z;
    public int w;

    public float4_Q3(float x, float y, float z, float w) {
        this.x = (int)math.round(x * MULTIPLIER);
        this.y = (int)math.round(y * MULTIPLIER);
        this.z = (int)math.round(z * MULTIPLIER);
        this.w = (int)math.round(w * MULTIPLIER);
    }

    public float4_Q3(float xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

    public float4_Q3(int x, int y, int z, int w) {
        this.x = x * (int)MULTIPLIER;
        this.y = y * (int)MULTIPLIER;
        this.z = z * (int)MULTIPLIER;
        this.w = w * (int)MULTIPLIER;
    }

    public float4_Q3(int xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

    public bool Equals(float4_Q3 other) =>
        x == other.x
     && y == other.y
     && z == other.z
     && w == other.w;

    public override string ToString() => ((float4)this).ToString();

    #region CAST

    public static explicit operator float4_Q3(float4 source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator float4(float4_Q3 source) => new(
        source.x / MULTIPLIER
      , source.y / MULTIPLIER
      , source.z / MULTIPLIER
      , source.w / MULTIPLIER);

    public static explicit operator float4_Q3(Vector4 source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator Vector4(float4_Q3 source) => new(
        source.x / MULTIPLIER
      , source.y / MULTIPLIER
      , source.z / MULTIPLIER
      , source.w / MULTIPLIER);

    public static explicit operator float4_Q3(quaternion source) => new(
        source.value.x
      , source.value.y
      , source.value.z
      , source.value.w);

    public static implicit operator quaternion(float4_Q3 source) => new(
        source.x / MULTIPLIER
      , source.y / MULTIPLIER
      , source.z / MULTIPLIER
      , source.w / MULTIPLIER);

    public static explicit operator float4_Q3(Quaternion source) => new(
        source.x
      , source.y
      , source.z
      , source.w);

    public static implicit operator Quaternion(float4_Q3 source) => new(
        source.x / MULTIPLIER
      , source.y / MULTIPLIER
      , source.z / MULTIPLIER
      , source.w / MULTIPLIER);

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

    public static float4_Q3 operator -(float4_Q3 source) => new() {
        x = -source.x
      , y = -source.y
      , z = -source.z
      , w = -source.w
    };

    public static float4_Q3 operator *(float4_Q3 a, int mul) => new() {
        x = a.x * mul
      , y = a.y * mul
      , z = a.z * mul
      , w = a.w * mul
    };

    public static float4_Q3 operator *(float4_Q3 a, float mul) => new() {
        x = (int)math.round(a.x * mul)
      , y = (int)math.round(a.y * mul)
      , z = (int)math.round(a.z * mul)
      , w = (int)math.round(a.w * mul)
    };

    public static float4_Q3 operator /(float4_Q3 a, int div) => new() {
        x = (int)math.round(a.x / (float)div)
      , y = (int)math.round(a.y / (float)div)
      , z = (int)math.round(a.z / (float)div)
      , w = (int)math.round(a.w / (float)div)
    };

    public static float4_Q3 operator /(float4_Q3 a, float div) => new() {
        x = (int)math.round(a.x / div)
      , y = (int)math.round(a.y / div)
      , z = (int)math.round(a.z / div)
      , w = (int)math.round(a.w / div)
    };

    #endregion

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(float4_Q3))]
    private class float4_Q3Drawer : PropertyDrawer {
        private Dictionary<string, InstanceDrawer> Properties { get; } = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string path = property.propertyPath;
            if (!Properties.ContainsKey(path)) Properties.Add(path, new(property));
            var drawer = Properties[path];

            position.height = EditorGUIUtility.singleLineHeight;
            drawer.Value = EditorGUI.Vector3Field(
                position
              , label
              , drawer.Value);
        }

        private class InstanceDrawer {
            private readonly SerializedProperty x, y, z, w;
            private          Vector4            _Value;

            public Vector4 Value {
                get => _Value;
                set {
                    var newValue = (float4_Q3)(_Value = value);
                    x.intValue = newValue.x;
                    y.intValue = newValue.y;
                    z.intValue = newValue.z;
                    w.intValue = newValue.w;
                }
            }

            public InstanceDrawer(SerializedProperty property) {
                x = property.FindPropertyRelative(nameof(float4_Q3.x));
                y = property.FindPropertyRelative(nameof(float4_Q3.y));
                z = property.FindPropertyRelative(nameof(float4_Q3.z));
                w = property.FindPropertyRelative(nameof(float4_Q3.w));

                _Value = new float4_Q3() {
                    x = x.intValue
                  , y = y.intValue
                  , z = z.intValue
                  , w = w.intValue
                };
            }
        }
    }

    #endif
}
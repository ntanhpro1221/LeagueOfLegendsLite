using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct floatXZ_Q3 : IEquatable<floatXZ_Q3> {
    public const float MULTIPLIER = 1000;

    public static readonly floatXZ_Q3 identity = new() { z = 1 };
    public static readonly floatXZ_Q3 zero     = new();

    public bool IsZero    => Equals(zero);
    public void SetZero() => (x, z) = (0, 0);

    public int x;
    public int z;

    public float3_Q3 Full => new() {
        x = x
      , z = z
    };

    public floatXZ_Q3(float x, float z) {
        this.x = Mathf.RoundToInt(x * MULTIPLIER);
        this.z = Mathf.RoundToInt(z * MULTIPLIER);
    }

    public floatXZ_Q3(float xz) : this(xz, xz) { }

    public floatXZ_Q3(int x, int z) {
        this.x = x * (int)MULTIPLIER;
        this.z = z * (int)MULTIPLIER;
    }

    public floatXZ_Q3(int xz) : this(xz, xz) { }

    public bool Equals(floatXZ_Q3 other) =>
        x == other.x
     && z == other.z;

    public override string ToString() => ((float2)this).ToString();

    public override int GetHashCode() => HashCode.Combine(x, z);

#region CAST

    public static explicit operator floatXZ_Q3(float2 source) => new(
        source.x
      , source.y);

    public static implicit operator float2(floatXZ_Q3 source) => new(
        source.x / MULTIPLIER
      , source.z / MULTIPLIER);

    public static explicit operator floatXZ_Q3(Vector2 source) => new(
        source.x
      , source.y);

    public static implicit operator Vector2(floatXZ_Q3 source) => new(
        source.x / MULTIPLIER
      , source.z / MULTIPLIER);

#endregion

#region OPERATOR

    public static floatXZ_Q3 operator +(floatXZ_Q3 a, floatXZ_Q3 b) => new() {
        x = a.x + b.x
      , z = a.z + b.z
    };

    public static floatXZ_Q3 operator -(floatXZ_Q3 a, floatXZ_Q3 b) => new() {
        x = a.x - b.x
      , z = a.z - b.z
    };

    public static floatXZ_Q3 operator -(floatXZ_Q3 source) => new() {
        x = -source.x
      , z = -source.z
    };

    public static floatXZ_Q3 operator *(floatXZ_Q3 a, int mul) => new() {
        x = a.x * mul
      , z = a.z * mul
    };

    public static floatXZ_Q3 operator *(floatXZ_Q3 a, float mul) => new() {
        x = Mathf.RoundToInt(a.x * mul)
      , z = Mathf.RoundToInt(a.z * mul)
    };

    public static floatXZ_Q3 operator /(floatXZ_Q3 a, int div) => new() {
        x = Mathf.RoundToInt(a.x / (float)div)
      , z = Mathf.RoundToInt(a.z / (float)div)
    };

    public static floatXZ_Q3 operator /(floatXZ_Q3 a, float div) => new() {
        x = Mathf.RoundToInt(a.x / div)
      , z = Mathf.RoundToInt(a.z / div)
    };

#endregion

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(floatXZ_Q3))]
    private class floatXZ_Q3Drawer : PropertyDrawer {
        private Dictionary<string, InstanceDrawer> Properties { get; } = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            string path = property.propertyPath;
            if (!Properties.ContainsKey(path)) Properties.Add(path, new(property));
            var drawer = Properties[path];

            position.height = EditorGUIUtility.singleLineHeight;
            drawer.Value = EditorGUI.Vector2Field(
                position
              , label
              , drawer.Value);
        }

        private class InstanceDrawer {
            private readonly SerializedProperty x, z;
            private          Vector2            _Value;

            public Vector2 Value {
                get => _Value;
                set {
                    var newValue = (floatXZ_Q3)(_Value = value);
                    x.intValue = newValue.x;
                    z.intValue = newValue.z;
                }
            }

            public InstanceDrawer(SerializedProperty property) {
                x = property.FindPropertyRelative(nameof(floatXZ_Q3.x));
                z = property.FindPropertyRelative(nameof(floatXZ_Q3.z));

                _Value = new floatXZ_Q3() {
                    x = x.intValue
                  , z = z.intValue
                };
            }
        }
    }

    #endif
}
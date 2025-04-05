using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        this.x = Mathf.RoundToInt(x * MULTIPLIER);
        this.y = Mathf.RoundToInt(y * MULTIPLIER);
        this.z = Mathf.RoundToInt(z * MULTIPLIER);
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

    public override string ToString() => ((float3)this).ToString();

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

    public static float3_Q3 operator -(float3_Q3 source) => new() {
        x = -source.x
      , y = -source.y
      , z = -source.z
    };

    public static float3_Q3 operator *(float3_Q3 a, int mul) => new() {
        x = a.x * mul
      , y = a.y * mul
      , z = a.z * mul
    };

    public static float3_Q3 operator *(float3_Q3 a, float mul) => new() {
        x = Mathf.RoundToInt(a.x * mul)
      , y = Mathf.RoundToInt(a.y * mul)
      , z = Mathf.RoundToInt(a.z * mul)
    };

    public static float3_Q3 operator /(float3_Q3 a, int div) => new() {
        x = a.x / div
      , y = a.y / div
      , z = a.z / div
    };

    public static float3_Q3 operator /(float3_Q3 a, float div) => new() {
        x = Mathf.RoundToInt(a.x / div)
      , y = Mathf.RoundToInt(a.y / div)
      , z = Mathf.RoundToInt(a.z / div)
    };

    #endregion

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(float3_Q3))]
    private class float3_Q3Drawer : PropertyDrawer {
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
            private readonly SerializedProperty x, y, z;
            private          Vector3            _Value;

            public Vector3 Value {
                get => _Value;
                set {
                    var newValue = (float3_Q3)(_Value = value);
                    x.intValue = newValue.x;
                    y.intValue = newValue.y;
                    z.intValue = newValue.z;
                }
            }

            public InstanceDrawer(SerializedProperty property) {
                x = property.FindPropertyRelative(nameof(float3_Q3.x));
                y = property.FindPropertyRelative(nameof(float3_Q3.y));
                z = property.FindPropertyRelative(nameof(float3_Q3.z));

                _Value = new float3_Q3() {
                    x = x.intValue
                  , y = y.intValue
                  , z = z.intValue
                };
            }
        }
    }

    #endif
}
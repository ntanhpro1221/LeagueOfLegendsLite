using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct float_Q3 : IEquatable<float_Q3> {
    public const int MULTIPLITER = 1000;
    public const float Epsilon = 0.001f;

    public int value;

    public float_Q3(float value) {
        this.value = Mathf.RoundToInt(value * MULTIPLITER);
    }
    
    public float_Q3(int value) {
        this.value = value * MULTIPLITER;
    }

    public bool Equals(float_Q3 other) =>
        value == other.value;

    #region CAST
    
    public static explicit operator float_Q3(float source) =>
        new(source);

    public static implicit operator float(float_Q3 source) =>
        (float)source.value / MULTIPLITER;
    
    public static implicit operator float_Q3(int source) =>
        new(source);

    public static explicit operator int(float_Q3 source) =>
        source.value / MULTIPLITER;
    
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
        value = Mathf.RoundToInt(a.value * mul)
    };

    public static float_Q3 operator /(float_Q3 a, int div) => new() {
        value = a.value / div
    };

    public static float_Q3 operator /(float_Q3 a, float div) => new() {
        value = Mathf.RoundToInt(a.value / div)
    };
    
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
                MULTIPLITER
              * EditorGUI.FloatField(
                    position
                  , label
                  , (float)drawer.intValue / MULTIPLITER));
        }
    }

    #endif
}
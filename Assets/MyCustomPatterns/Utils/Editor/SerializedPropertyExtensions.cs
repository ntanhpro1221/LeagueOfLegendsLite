using System;
using UnityEditor;

namespace NGDtuanh.Utils.Editor {
    public static class SerializedPropertyExtensions {
        public static class Self {
            public static readonly Func<SerializedProperty, SerializedProperty> Getter = property => property;
        }
        
        public static class String {
            public static readonly Func<SerializedProperty, string>   Getter = property => property.stringValue;
            public static readonly Action<SerializedProperty, string> Setter = (property, value) => property.stringValue = value;
        }

        public static class Bool {
            public static readonly Func<SerializedProperty, bool>   Getter = property => property.boolValue;
            public static readonly Action<SerializedProperty, bool> Setter = (property, value) => property.boolValue = value;
        }

        public static class Int {
            public static readonly Func<SerializedProperty, int>   Getter = property => property.intValue;
            public static readonly Action<SerializedProperty, int> Setter = (property, value) => property.intValue = value;
        }

        public static class UInt {
            public static readonly Func<SerializedProperty, uint>   Getter = property => property.uintValue;
            public static readonly Action<SerializedProperty, uint> Setter = (property, value) => property.uintValue = value;
        }

        public static class Long {
            public static readonly Func<SerializedProperty, long>   Getter = property => property.longValue;
            public static readonly Action<SerializedProperty, long> Setter = (property, value) => property.longValue = value;
        }

        public static class ULong {
            public static readonly Func<SerializedProperty, ulong>   Getter = property => property.ulongValue;
            public static readonly Action<SerializedProperty, ulong> Setter = (property, value) => property.ulongValue = value;
        }

        public static class Float {
            public static readonly Func<SerializedProperty, float>   Getter = property => property.floatValue;
            public static readonly Action<SerializedProperty, float> Setter = (property, value) => property.floatValue = value;
        }

        public static class Double {
            public static readonly Func<SerializedProperty, double>   Getter = property => property.doubleValue;
            public static readonly Action<SerializedProperty, double> Setter = (property, value) => property.doubleValue = value;
        }

        public static class EnumIndex {
            public static readonly Func<SerializedProperty, int>   Getter = property => property.enumValueIndex;
            public static readonly Action<SerializedProperty, int> Setter = (property, value) => property.enumValueIndex = value;
        }

        public static class EnumFlag {
            public static readonly Func<SerializedProperty, int>   Getter = property => property.enumValueFlag;
            public static readonly Action<SerializedProperty, int> Setter = (property, value) => property.enumValueFlag = value;
        }

        public static class Rect {
            public static readonly Func<SerializedProperty, UnityEngine.Rect>   Getter = property => property.rectValue;
            public static readonly Action<SerializedProperty, UnityEngine.Rect> Setter = (property, value) => property.rectValue = value;
        }

        public static class RectInt {
            public static readonly Func<SerializedProperty, UnityEngine.RectInt>   Getter = property => property.rectIntValue;
            public static readonly Action<SerializedProperty, UnityEngine.RectInt> Setter = (property, value) => property.rectIntValue = value;
        }

        public static class Vector2 {
            public static readonly Func<SerializedProperty, UnityEngine.Vector2>   Getter = property => property.vector2Value;
            public static readonly Action<SerializedProperty, UnityEngine.Vector2> Setter = (property, value) => property.vector2Value = value;
        }

        public static class Vector2Int {
            public static readonly Func<SerializedProperty, UnityEngine.Vector2Int>   Getter = property => property.vector2IntValue;
            public static readonly Action<SerializedProperty, UnityEngine.Vector2Int> Setter = (property, value) => property.vector2IntValue = value;
        }

        public static class Vector3 {
            public static readonly Func<SerializedProperty, UnityEngine.Vector3>   Getter = property => property.vector3Value;
            public static readonly Action<SerializedProperty, UnityEngine.Vector3> Setter = (property, value) => property.vector3Value = value;
        }

        public static class Vector3Int {
            public static readonly Func<SerializedProperty, UnityEngine.Vector3Int>   Getter = property => property.vector3IntValue;
            public static readonly Action<SerializedProperty, UnityEngine.Vector3Int> Setter = (property, value) => property.vector3IntValue = value;
        }

        public static class Vector4 {
            public static readonly Func<SerializedProperty, UnityEngine.Vector4>   Getter = property => property.vector4Value;
            public static readonly Action<SerializedProperty, UnityEngine.Vector4> Setter = (property, value) => property.vector4Value = value;
        }

        public static class ObjectReference {
            public static readonly Func<SerializedProperty, UnityEngine.Object>   Getter = property => property.objectReferenceValue;
            public static readonly Action<SerializedProperty, UnityEngine.Object> Setter = (property, value) => property.objectReferenceValue = value;
        }

        public static class ManagedReference {
            public static readonly Func<SerializedProperty, object>   Getter = property => property.managedReferenceValue;
            public static readonly Action<SerializedProperty, object> Setter = (property, value) => property.managedReferenceValue = value;
        }

        public static class ExposedReference {
            public static readonly Func<SerializedProperty, UnityEngine.Object>   Getter = property => property.exposedReferenceValue;
            public static readonly Action<SerializedProperty, UnityEngine.Object> Setter = (property, value) => property.exposedReferenceValue = value;
        }

        public static class Color {
            public static readonly Func<SerializedProperty, UnityEngine.Color>   Getter = property => property.colorValue;
            public static readonly Action<SerializedProperty, UnityEngine.Color> Setter = (property, value) => property.colorValue = value;
        }

        public static class AnimationCurve {
            public static readonly Func<SerializedProperty, UnityEngine.AnimationCurve>   Getter = property => property.animationCurveValue;
            public static readonly Action<SerializedProperty, UnityEngine.AnimationCurve> Setter = (property, value) => property.animationCurveValue = value;
        }

        public static class Gradient {
            public static readonly Func<SerializedProperty, UnityEngine.Gradient>   Getter = property => property.gradientValue;
            public static readonly Action<SerializedProperty, UnityEngine.Gradient> Setter = (property, value) => property.gradientValue = value;
        }

        public static class LayerMask {
            public static readonly Func<SerializedProperty, int>   Getter = property => property.intValue;
            public static readonly Action<SerializedProperty, int> Setter = (property, value) => property.intValue = value;
        }

        public static class Quaternion {
            public static readonly Func<SerializedProperty, UnityEngine.Quaternion>   Getter = property => property.quaternionValue;
            public static readonly Action<SerializedProperty, UnityEngine.Quaternion> Setter = (property, value) => property.quaternionValue = value;
        }

        public static class Boxed {
            public static readonly Func<SerializedProperty, object>   Getter = property => property.boxedValue;
            public static readonly Action<SerializedProperty, object> Setter = (property, value) => property.boxedValue = value;
        }

        public static class Bounds {
            public static readonly Func<SerializedProperty, UnityEngine.Bounds>   Getter = property => property.boundsValue;
            public static readonly Action<SerializedProperty, UnityEngine.Bounds> Setter = (property, value) => property.boundsValue = value;
        }

        public static class BoundsInt {
            public static readonly Func<SerializedProperty, UnityEngine.BoundsInt>   Getter = property => property.boundsIntValue;
            public static readonly Action<SerializedProperty, UnityEngine.BoundsInt> Setter = (property, value) => property.boundsIntValue = value;
        }
    }
}
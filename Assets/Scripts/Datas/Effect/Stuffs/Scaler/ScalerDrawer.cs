#if UNITY_EDITOR

using System.Reflection;
using NGDtuanh.Collections.Editor;
using NGDtuanh.Utils;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Scaler))]
public class ScalerDrawer : IDrawer<ScalerDrawer.Instance> {
    public class Instance : IDrawerInstance {
        private SerializedProperty _Factor;
        private SerializedProperty _Source;
        private SerializedProperty _Stat;
        private SerializedProperty _Ratio;

        private bool ShowStat => _Factor.intValue == (int)Scaler.Factor.Stat;

        public override void Init(SerializedProperty property, FieldInfo fieldInfo) {
            _Factor = property.FindPropertyRelative(nameof(Scaler.factor));
            _Source = property.FindPropertyRelative(nameof(Scaler.source));
            _Stat   = property.FindPropertyRelative(nameof(Scaler.stat));
            _Ratio  = property.FindPropertyRelative(nameof(Scaler.ratio));
        }

        public override float GetPropertyHeight() {
            int lineCount = 3 + 1.IfOnly(ShowStat);

            return lineCount * LineHeight + Mathf.Max(0, lineCount - 1) * LineSpace;
        }

        public override void OnGUI(Rect position, GUIContent label) {
            float y = position.y;

            PropertyField(position, _Ratio,  ref y, firstLine: true);
            PropertyField(position, _Factor, ref y, firstLine: false);
            PropertyField(position, _Source, ref y, firstLine: false);
            PropertyField(position, _Stat,   ref y, firstLine: false, ShowStat);
        }

        private static void PropertyField(
            in Rect            position
          , SerializedProperty property
          , ref float          y
          , bool               firstLine
          , bool               ifOnly = true) {
            if (!ifOnly) return;

            EditorGUI.PropertyField(
                new Rect(position.x, y += (LineHeight + LineSpace).IfOnly(!firstLine), position.width, LineHeight)
              , property);
        }
    }
}

#endif
#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using NGDtuanh.Collections.Editor;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IEnableable), true)]
public class EnableableDrawer : IDrawer<EnableableDrawer.Instance> {
    public class Instance : IDrawerInstance {
        private SerializedProperty       _Enable;
        private List<SerializedProperty> _Props       = new();
        private List<float>              _PropsHeight = new();

        private bool _Foldout = true;
        private bool ShowProps => _Foldout && _Enable.boolValue;

        public override void Init(SerializedProperty property, FieldInfo fieldInfo) {
            _Enable = property.FindPropertyRelative(PropName(nameof(IEnableable.enable)));
            foreach (var prop in property.DirectChildren())
                if (!SerializedProperty.EqualContents(prop, _Enable)) {
                    _Props.Add(prop);
                    _PropsHeight.Add(0);
                }
        }

        public override float GetPropertyHeight() {
            float result = LineHeight;

            if (ShowProps) {
                for (int i = 0; i < _Props.Count; ++i)
                    result += _PropsHeight[i] = EditorGUI.GetPropertyHeight(_Props[i], true);
                result += _Props.Count * LineSpace;
            }

            return result;
        }

        public override void OnGUI(Rect position, GUIContent label) {
            var labelRect = position.With_Height(LineHeight);

            if (_Enable.boolValue)
                _Foldout  = EditorGUI.Foldout(labelRect, _Foldout, GUIContent.none);
            else _Foldout = true;
            
            EditorGUI.PropertyField(labelRect, _Enable, label);

            if (_Enable.boolValue && _Foldout) {
                ++EditorGUI.indentLevel;

                float y = position.y + LineHeight;
                for (int i = 0; i < _Props.Count; ++i) {
                    y += LineSpace;
                    EditorGUI.PropertyField(position.With_Y(y).With_Height(_PropsHeight[i]), _Props[i], true);
                    y += _PropsHeight[i];
                }

                --EditorGUI.indentLevel;
            }
        }
    }
}

#endif
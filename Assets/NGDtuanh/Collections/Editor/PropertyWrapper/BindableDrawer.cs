using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Collections.PropertyWrapper.Editor {
    [CustomPropertyDrawer(typeof(Bindable<>), true)]
    public class BindableDrawer : WrapperBaseDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndProperty();
        }
    }
}
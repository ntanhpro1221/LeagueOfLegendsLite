using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Collections.PropertyWrapper.Editor {
    [CustomPropertyDrawer(typeof(WrapperBase<>), true)]
    public class WrapperBaseDrawer : PropertyDrawer {
        private readonly Dictionary<string, SerializedProperty> _CachedValue = new();

        protected SerializedProperty GetSafeCachedValue(SerializedProperty property) {
            if (!_CachedValue.ContainsKey(property.propertyPath))
                _CachedValue.Add(property.propertyPath, property.FindPropertyRelative(WrapperBase<int>.ValueSerializeName));

            return _CachedValue[property.propertyPath];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(GetSafeCachedValue(property));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var valueProp = GetSafeCachedValue(property);
            EditorGUI.BeginProperty(position, label, valueProp);
            
            EditorGUI.PropertyField(position, valueProp, label, true);

            EditorGUI.EndProperty();
        }
    }
}
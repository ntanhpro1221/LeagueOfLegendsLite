using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEditor;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    [CustomPropertyDrawer(typeof(EnumMap<,>), true)]
    public class EnumMapDrawer : PropertyDrawer {
        
        private readonly Dictionary<string, EnumMapInstanceDrawer> _InstanceDrawers = new();

        private EnumMapInstanceDrawer EnsureGetInstanceDrawer(SerializedProperty property) {
            if (!_InstanceDrawers.ContainsKey(property.propertyPath))
                _InstanceDrawers.Add(property.propertyPath, new(property, fieldInfo));

            return _InstanceDrawers[property.propertyPath];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EnsureGetInstanceDrawer(property).GetPropertyHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EnsureGetInstanceDrawer(property).OnGUI(position, label);
        }
    }
}
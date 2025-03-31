using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEditor;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    [CustomPropertyDrawer(typeof(EnumMap<,>), true)]
    public class EnumMapDrawer : PropertyDrawer {
        
        private readonly Dictionary<string, EnumMapInstanceDrawer> _InstanceDrawers = new();

        private EnumMapInstanceDrawer EnsureGetInstanceDrawer(SerializedProperty property) {
            // REMEMBER DONT FALL TO THIS **** "TRYADD()" AGAIN 🥲🥲
            // (MEMORY LEAK!!!)
            // _InstanceDrawers.TryAdd(property.propertyPath, new(property, fieldInfo));
            // return _InstanceDrawers[property.propertyPath];
            
            var path = property.propertyPath;
            if (!_InstanceDrawers.ContainsKey(path))
                _InstanceDrawers.Add(path, new(property, fieldInfo));
            
            return _InstanceDrawers[path];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EnsureGetInstanceDrawer(property).GetPropertyHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EnsureGetInstanceDrawer(property).OnGUI(position, label);
        }
    }
}
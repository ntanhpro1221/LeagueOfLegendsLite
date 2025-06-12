using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Collections.Editor {
    public abstract class IDrawer<TInstance> : PropertyDrawer where TInstance : IDrawerInstance, new() {
        private readonly Dictionary<string, TInstance> _Instances = new();

        private TInstance GetInstance(SerializedProperty property) {
            var path = property.propertyPath;
            if (!_Instances.ContainsKey(path)) {
                var newInstance = new TInstance();
                newInstance.Init(property, fieldInfo);
                _Instances.Add(path, newInstance);
            }
            
            return _Instances[path];
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return GetInstance(property).GetPropertyHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GetInstance(property).OnGUI(position, label);
        }
    }
}
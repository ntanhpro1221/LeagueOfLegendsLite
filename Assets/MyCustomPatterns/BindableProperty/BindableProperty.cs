using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BindablePropertyUtil {
    /// <summary>
    /// Invoke your action when its value is changed
    /// </summary>
    [Serializable]
    public class BindableProperty<T> {
        [FormerlySerializedAs("_Value")] [SerializeField]
        private T _Value;

        public UnityEvent<T> OnChanged { get; } = new();

        public T Value {
            get => _Value;
            set {
                if (_Value == null ? value != null : !_Value.Equals(value))
                    OnChanged.Invoke(_Value = value);
            }
        }

        public override string ToString()
            => _Value.ToString();

        public static implicit operator string(BindableProperty<T> obj)
            => obj.ToString();
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BindableProperty<>), true)]
    public class BindablePropertyDrawer : PropertyDrawer {
        private SerializedProperty _Value;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            _Value = property.FindPropertyRelative(nameof(_Value));
            return EditorGUI.GetPropertyHeight(_Value);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUI.GetPropertyHeight(_Value);
            EditorGUI.PropertyField(position, _Value, new(property.displayName), true);
        }
    }
#endif
}

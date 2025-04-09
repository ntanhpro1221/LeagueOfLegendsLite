using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HDRColor))]
public class HDRColorDrawer : PropertyDrawer {
    private Dictionary<string, SerializedProperty> Instances = new();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        string path = property.propertyPath;
        if (!Instances.ContainsKey(path))
            Instances.Add(path, property.FindPropertyRelative(nameof(HDRColor.Value)));

        EditorGUI.PropertyField(position, Instances[path], label);
    }
}
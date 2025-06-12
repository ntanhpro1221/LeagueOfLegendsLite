using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Collections.Editor {
    public abstract class IDrawerInstance {
#region CONVINIENT CONSTANT

        public static float LineHeight => EditorGUIUtility.singleLineHeight;
        public static float LineSpace => EditorGUIUtility.standardVerticalSpacing;

#endregion

#region CONVINIENT FUNCTION

        public static string PropName(string fieldName) => $"<{fieldName}>k__BackingField";
        
#endregion
        
        public abstract void Init(SerializedProperty property, FieldInfo fieldInfo);

        public abstract float GetPropertyHeight();

        public abstract void OnGUI(Rect position, GUIContent label);
    }
}
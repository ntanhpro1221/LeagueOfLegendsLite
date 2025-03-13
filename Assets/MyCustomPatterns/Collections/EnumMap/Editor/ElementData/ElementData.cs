using NGDtuanh.Collections.PropertyWrapper;
using NGDtuanh.Utils;
using UnityEditor;
using UnityEngine;

namespace MyCustomPatterns.Collections.EnumMap.Editor {
    public class ElementData {
        public readonly SerializedProperty Key;
        public readonly SerializedProperty Value;
        public readonly SerializedProperty Value_InsideWrapper;
        public readonly GUIContent         Label;
        public          bool               Visible;

        private readonly AllElementDatas AllElementDatas;

        public ElementData(SerializedProperty key
                         , SerializedProperty value
                         , GUIContent         label
                         , bool               visible
                         , AllElementDatas    allElementDatas) {
            Key                 = key;
            Value               = value;
            Value_InsideWrapper = value.FindPropertyRelative(WrapperBase<int>.ValueSerializeName);
            Label               = label;
            Visible             = visible;
            AllElementDatas     = allElementDatas;
        }

        public float GetHeight() {
            if (!Visible) return 0;

            return
                EditorGUI.GetPropertyHeight(Value)
              + AllElementDatas.ElementHeightDel;
        }

        public void Draw(in Rect position) {
            if (!Visible) return;
            
            ++EditorGUI.indentLevel;

            EditorGUI.PropertyField(
                position.With_Padding(AllElementDatas.ElementPadding)
              , Value
              , Label
              , true);
            
            --EditorGUI.indentLevel;
        }
    }
}
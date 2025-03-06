using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "MyData", menuName = "Data/My Scriptable Object")]
public class MyScriptableObject : ScriptableObject {
    public int    number;
    public string description;
}


[CustomEditor(typeof(MyScriptableObject))]
public class MyScriptableObjectEditor : Editor {
    public override void OnInspectorGUI() {
        // Cập nhật serializedObject để làm việc với SerializedProperty
        serializedObject.Update();

        // Lấy SerializedProperty tương ứng với từng field
        SerializedProperty numberProp      = serializedObject.FindProperty("number");
        SerializedProperty descriptionProp = serializedObject.FindProperty("description");

        // Vẽ các field theo ý muốn (có thể sử dụng EditorGUILayout)
        EditorGUILayout.LabelField("Custom Inspector", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(numberProp,      new GUIContent("Số"));
        EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Mô tả"));

        // Xử lý các logic khác nếu cần...

        // Áp dụng các thay đổi từ SerializedObject
        serializedObject.ApplyModifiedProperties();
    }
}

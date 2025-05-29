namespace NGDtuanh.UI.Editor {
    using UnityEditor;

    [CustomEditor(typeof(LayoutElementRatio))]
    [CanEditMultipleObjects]
    public class LayoutElementRatioEditor : Editor {
        private SerializedProperty m_AspectMode;
        private SerializedProperty m_Padding;
        private SerializedProperty m_AspectRatio;
        private SerializedProperty m_LayoutPriority;

        private void OnEnable() {
            m_AspectMode     = serializedObject.FindProperty(nameof(m_AspectMode));
            m_Padding        = serializedObject.FindProperty(nameof(m_Padding));
            m_AspectRatio    = serializedObject.FindProperty(nameof(m_AspectRatio));
            m_LayoutPriority = serializedObject.FindProperty(nameof(m_LayoutPriority));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_AspectMode);
            EditorGUILayout.PropertyField(m_Padding);
            EditorGUILayout.PropertyField(m_AspectRatio);
            EditorGUILayout.PropertyField(m_LayoutPriority);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
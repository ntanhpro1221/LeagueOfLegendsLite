using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace PropertySetUtil {
    /// <summary>
    /// Set of value with the same type. Each element can be access easier through enum key
    /// </summary>
    /// <typeparam name="TKey">Enum type of key</typeparam>
    /// <typeparam name="TValue">Data type of each element</typeparam>
    [Serializable]
    public class PropertySet<TKey, TValue> :
        Dictionary<string, TValue>, ISerializationCallbackReceiver
        where TKey : Enum {
        /// <summary>
        /// to get enum type
        /// </summary>
        [FormerlySerializedAs("m_KeyType")] [SerializeField]
        private TKey _KeyType;

        /// <summary>
        /// to restore value with corresponding key when enum's script change
        /// </summary>
        [FormerlySerializedAs("m_Keys")] [SerializeField]
        private string[] _Keys;

        [FormerlySerializedAs("m_Values")] [SerializeField]
        private TValue[] _Values;

        public new string[] Keys   => _Keys;
        public new TValue[] Values => _Values;

        private void InitEnumField() {
            _Keys   = Enum.GetNames(typeof(TKey));
            _Values = new TValue[Enum.GetNames(typeof(TKey)).Length];
            for (int i = 0; i < _Values.Length; i++) _Values[i] = default;
        }

        public PropertySet() : base() => InitEnumField();

        private int EToInt(TKey key) => Convert.ToInt32(key);

        public TValue this[TKey key] {
            get => _Values[EToInt(key)];
            set => _Values[EToInt(key)] = value;
        }

        public void OnBeforeSerialize() {
            if (_Keys.Length == 0) InitEnumField();
            foreach (TKey key in Enum.GetValues(typeof(TKey)))
                if (base.Keys.Contains(key.ToString()))
                    this[key] = base[key.ToString()];
        }

        public void OnAfterDeserialize() {
            if (_Keys.Length == 0) InitEnumField();
            base.Clear();
            foreach (TKey key in Enum.GetValues(typeof(TKey)))
                base.Add(key.ToString(), this[key]);
        }

        public new TValue this[string key] {
            get => throw new Exception("Dont use this function!!!");
            set => throw new Exception("Dont use this function!!!");
        }

        public new void Add(string key, TValue value)
            => throw new Exception("Dont use this function!!!");

        public new bool TryAdd(string key, TValue value)
            => throw new Exception("Dont use this function!!!");

        public new void Clear()
            => throw new Exception("Dont use this function!!!");

        public new bool Remove(string key)
            => throw new Exception("Dont use this function!!!");

        public new bool Remove(string key, out TValue value)
            => throw new Exception("Dont use this function!!!");
    }

#if UNITY_EDITOR
    /// <summary>
    /// Enable edit PropertySet in Inspector
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertySet<,>), true)]
    public class PropertySetDrawer : PropertyDrawer {
        private SerializedProperty _KeyType;
        private SerializedProperty _Keys;
        private SerializedProperty _Values;
        private bool               _Dirty;

        private static float LineHeight => EditorGUIUtility.singleLineHeight;
        private static float LineSpace  => 2;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            _Dirty = true;
            return base.CreatePropertyGUI(property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            _KeyType = property.FindPropertyRelative(nameof(_KeyType));
            _Keys    = property.FindPropertyRelative(nameof(_Keys));
            _Values  = property.FindPropertyRelative(nameof(_Values));
            if (_Dirty) Clean();

            var height = LineHeight;
            if (property.isExpanded)
                for (int i = 0; i < _KeyType.enumNames.Length; ++i)
                    height +=
                        EditorGUI.GetPropertyHeight(_Values.GetArrayElementAtIndex(i)) +
                        (i + 1 != _KeyType.enumNames.Length ? LineSpace : 0);
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            Rect labelPosition = new(position.x, position.y, position.width, LineHeight);
            property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label, true);
            if (property.isExpanded) {
                ++EditorGUI.indentLevel;
                ++EditorGUI.indentLevel;
                float addY = LineHeight;
                for (int i = 0; i < _KeyType.enumNames.Length; ++i)
                    EditorGUI.PropertyField(
                        new Rect(
                            position.x,
                            position.y + addY,
                            position.width,
                            addY +=
                                EditorGUI.GetPropertyHeight(_Values.GetArrayElementAtIndex(i)) +
                                (i + 1 != _KeyType.enumNames.Length ? LineSpace : 0)),
                        _Values.GetArrayElementAtIndex(i),
                        new GUIContent(_KeyType.enumNames[i]),
                        true);
                --EditorGUI.indentLevel;
                --EditorGUI.indentLevel;
            }

            EditorGUI.EndProperty();
        }

        private bool CheckEnumKeyIntact() {
            if (_Keys.arraySize != _KeyType.enumNames.Length) return false;
            for (int i = 0; i < _KeyType.enumNames.Length; ++i)
                if (false == _Keys.GetArrayElementAtIndex(i).stringValue.Equals(_KeyType.enumNames[i]))
                    return false;
            return true;
        }

        private void UpdateEnumKey() {
            // Key algorithm: foreach element in new enum key, restore value if this key exist in old enum key

            // add more element if size of the new enum key is BIGGER then old one
            // (don't remove element now in the opposite case and assign empty to new key string) because we need restore old value (if possible)
            while (_Keys.arraySize < _KeyType.enumNames.Length) {
                _Keys.InsertArrayElementAtIndex(0);
                _Values.InsertArrayElementAtIndex(0);
                _Keys.GetArrayElementAtIndex(0).stringValue = string.Empty;
            }

            // restore value of old enum key (if possible)
            for (int i = 0; i < _KeyType.enumNames.Length; ++i) {
                int? matchID = null;
                for (int j = 0; j < _Keys.arraySize; ++j)
                    if (_Keys.GetArrayElementAtIndex(j).stringValue.Equals(_KeyType.enumNames[i]))
                        matchID = j;

                if (matchID == null || matchID == i) continue;

                int left                        = i, right = (int)matchID;
                if (left > right) (left, right) = (right, left);
                _Keys.MoveArrayElement(left, right);
                _Values.MoveArrayElement(left, right);
                _Keys.MoveArrayElement(right   - 1, left);
                _Values.MoveArrayElement(right - 1, left);
            }

            // remove element if if size of the new enum key is SMALLER then old one
            while (_Keys.arraySize > _KeyType.enumNames.Length) {
                int lastID = _Keys.arraySize - 1;
                _Keys.DeleteArrayElementAtIndex(lastID);
                _Values.DeleteArrayElementAtIndex(lastID);
            }

            // assign correct enum key
            for (int i = 0; i < _Keys.arraySize; ++i)
                _Keys.GetArrayElementAtIndex(i).stringValue = _KeyType.enumNames[i];
        }

        private void Clean() {
            _Dirty = false;
            if (CheckEnumKeyIntact() == false) UpdateEnumKey();
        }
    }
#endif
}

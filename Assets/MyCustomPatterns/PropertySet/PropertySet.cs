using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PropertySetUtil {
    /// <summary>
    /// Set of value with the same type. Each element can be access easier through enum key
    /// </summary>
    /// <typeparam name="TKey">Enum type of key</typeparam>
    /// <typeparam name="TValue">Data type of each element</typeparam>
    [Serializable]
    public class PropertySet<TKey, TValue> :
        Dictionary<string, TValue>
      , ISerializationCallbackReceiver
      , IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : Enum {
        [SerializeField]
        private TKey _KeyType;

        [SerializeField]
        private TKey[] _TrueKeys;

        [SerializeField]
        private TValue[] _Values;

        private Dictionary<TKey, long>   _Key2Long;
        private Dictionary<TKey, string> _Key2String;

        public new int      Count  => _TrueKeys.Length;
        public new TKey[]   Keys   => _TrueKeys.ToArray();
        public new TValue[] Values => _Values.ToArray();

        #if UNITY_EDITOR
        [SerializeField] [HideInInspector] private string[] _PrevKeyNames;
        [SerializeField] [HideInInspector] private long[]   _PrevKeyValues;
        #endif

        private void InitEnumField() {
            _TrueKeys = (TKey[])Enum.GetValues(typeof(TKey));
            _Values   = new TValue[_TrueKeys.Length];
            #if UNITY_EDITOR
            _PrevKeyNames  = Enum.GetNames(typeof(TKey));
            _PrevKeyValues = _TrueKeys.Select(key => Convert.ToInt64(key)).ToArray();
            #endif
        }

        private void MergeToBaseAndKeyTable() {
            base.Clear();
            _Key2Long   = new();
            _Key2String = new();
            for (int i = 0; i < _TrueKeys.Length; ++i) {
                base.Add(_TrueKeys[i].ToString(), _Values[i]);
                _Key2Long.Add(_TrueKeys[i], i);
                _Key2String.Add(_TrueKeys[i], _TrueKeys[i].ToString());
            }
        }

        public PropertySet() {
            InitEnumField();
            MergeToBaseAndKeyTable();
        }

        public TValue this[TKey key] {
            get => _Values[_Key2Long[key]];
            set => _Values[_Key2Long[key]] = base[_Key2String[key]] = value;
        }

        public void OnBeforeSerialize() {
            if (_TrueKeys.Length == 0) InitEnumField();
            else _TrueKeys = (TKey[])Enum.GetValues(typeof(TKey));
            foreach (TKey key in _TrueKeys)
                if (base.Keys.Contains(key.ToString()))
                    this[key] = base[key.ToString()];
        }

        public void OnAfterDeserialize() {
            MergeToBaseAndKeyTable();
        }

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new(_TrueKeys[i], _Values[i]);
        }

        #region Hide Base's Method

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

        #endregion
    }
    #if UNITY_EDITOR
    /// <summary>
    /// Enable edit PropertySet in Inspector
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertySet<,>), true)]
    public class PropertySetDrawer : PropertyDrawer {
        #region CONFIG UI VAR

        private static float LineHeight => EditorGUIUtility.singleLineHeight;
        private static float LineSpace  => 2;

        #endregion

        #region PROPERTY

        private SerializedProperty _KeyType;
        private SerializedProperty _TrueKeys;
        private SerializedProperty _Values;
        private SerializedProperty _PrevKeyNames;
        private SerializedProperty _PrevKeyValues;

        #endregion

        #region UTILS

        // KEY NAME
        private List<string> _PrevKeyNamesUtil;
        private List<string> _AlwaysTrueKeyNames;

        // KEY VALUE (long)
        private List<long?> _PrevKeyValuesUtil;
        private List<long>  _AlwaysTrueKeyValues;

        // VALUE
        private List<float>              _ValuesHeight;
        private List<SerializedProperty> _ValuesChild;

        // KEY COUNT
        private int _AlwaysTrueKeyCount;

        void Swap<T>(ref T left, ref T right)
            => (left, right) = (right, left);

        void SwapList<T>(List<T> list, int leftId, int rightId)
            => (list[leftId], list[rightId]) = (list[rightId], list[leftId]);

        void SwapFullValue(int leftId, int rightId) {
            if (leftId > rightId) Swap(ref leftId, ref rightId);
            _Values.MoveArrayElement(leftId,      rightId);
            _Values.MoveArrayElement(rightId - 1, leftId);
            SwapList(_PrevKeyNamesUtil,  leftId, rightId);
            SwapList(_PrevKeyValuesUtil, leftId, rightId);
        }

        #endregion

        private bool _Dirty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            _Dirty = true;
            return base.CreatePropertyGUI(property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Assign property value
            _KeyType       = property.FindPropertyRelative(nameof(_KeyType));
            _TrueKeys      = property.FindPropertyRelative(nameof(_TrueKeys));
            _Values        = property.FindPropertyRelative(nameof(_Values));
            _PrevKeyNames  = property.FindPropertyRelative(nameof(_PrevKeyNames));
            _PrevKeyValues = property.FindPropertyRelative(nameof(_PrevKeyValues));

            // Assign utilities variable
            _PrevKeyNamesUtil = Enumerable
                .Range(0, _PrevKeyNames.arraySize)
                .Select(id => _PrevKeyNames.GetArrayElementAtIndex(id).stringValue)
                .ToList();
            _PrevKeyValuesUtil = Enumerable
                .Range(0, _PrevKeyValues.arraySize)
                .Select(id => (long?)_PrevKeyValues.GetArrayElementAtIndex(id).longValue)
                .ToList();
            _AlwaysTrueKeyNames = _KeyType.enumNames
                .ToList();
            _AlwaysTrueKeyCount = _AlwaysTrueKeyNames
                .Count;
            _AlwaysTrueKeyValues = Enumerable
                .Range(0, _AlwaysTrueKeyCount)
                .Select(id => _TrueKeys.GetArrayElementAtIndex(id).longValue)
                .ToList();

            while (_PrevKeyNames.arraySize < _AlwaysTrueKeyCount) {
                _PrevKeyNames.InsertArrayElementAtIndex(0);
                _PrevKeyValues.InsertArrayElementAtIndex(0);
            }

            for (int i = 0; i < _AlwaysTrueKeyCount; ++i) {
                _PrevKeyNames.GetArrayElementAtIndex(i).stringValue = _AlwaysTrueKeyNames[i];
                _PrevKeyValues.GetArrayElementAtIndex(i).longValue  = _AlwaysTrueKeyValues[i];
            }

            // Reassign values if there is some enum's script change
            if (_Dirty) Clean();

            _ValuesChild = Enumerable
                .Range(0, _AlwaysTrueKeyCount)
                .Select(_Values.GetArrayElementAtIndex)
                .ToList();
            _ValuesHeight = _ValuesChild
                .Select(EditorGUI.GetPropertyHeight)
                .ToList();

            // Calculate height
            return LineHeight + (property.isExpanded
                ? LineSpace * Mathf.Max(0, _AlwaysTrueKeyCount - 1) + _ValuesHeight.Sum()
                : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            Rect labelPosition = new(position.x, position.y, position.width, LineHeight);
            property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label, true);
            if (property.isExpanded) {
                ++EditorGUI.indentLevel;
                ++EditorGUI.indentLevel;
                var addY = LineHeight;
                for (int i = 0; i < _AlwaysTrueKeyCount; ++i)
                    EditorGUI.PropertyField(
                        new Rect(
                            position.x,
                            position.y + addY,
                            position.width,
                            addY +=
                                _ValuesHeight[i]
                              + (i + 1 != _AlwaysTrueKeyCount ? LineSpace : 0)),
                        _ValuesChild[i],
                        new GUIContent(_AlwaysTrueKeyNames[i]),
                        true);
                --EditorGUI.indentLevel;
                --EditorGUI.indentLevel;
            }

            EditorGUI.EndProperty();
        }

        private bool CheckEnumKeyIntact() =>
            _PrevKeyNamesUtil.SequenceEqual(_AlwaysTrueKeyNames);

        private void UpdateEnumKey() {
            // add element (if new size > old size)
            while (_Values.arraySize < _AlwaysTrueKeyCount) {
                int lastId = _Values.arraySize - 1;
                _Values.InsertArrayElementAtIndex(lastId);
                _PrevKeyNamesUtil.Add(null);
                _PrevKeyValuesUtil.Add(null);
            }

            // restore value of old enum key (if possible)
            for (int i = 0; i < _AlwaysTrueKeyCount; ++i) {
                int matchID = _PrevKeyNamesUtil.IndexOf(_AlwaysTrueKeyNames[i]);
                
                if (matchID == -1 || matchID == i) continue;
                
                SwapFullValue(i, matchID);
            }

            // restore value of old enum value (if possible and not be restored by name yet)
            for (int i = 0; i < _AlwaysTrueKeyCount; ++i) {
                if (_PrevKeyNamesUtil[i] == _AlwaysTrueKeyNames[i]) continue;

                int matchID = _PrevKeyValuesUtil.IndexOf(_AlwaysTrueKeyValues[i]);

                if (matchID == -1 || matchID == i) continue;

                if (matchID                    < _AlwaysTrueKeyCount
                 && _PrevKeyNamesUtil[matchID] == _AlwaysTrueKeyNames[matchID])
                    continue;

                SwapFullValue(i, matchID);
            }

            // remove element (if new size < old size)
            while (_Values.arraySize < _AlwaysTrueKeyCount) {
                int lastId = _Values.arraySize - 1;
                _Values.DeleteArrayElementAtIndex(lastId);
                _PrevKeyNamesUtil.RemoveAt(lastId);
                _PrevKeyValuesUtil.RemoveAt(lastId);
            }
        }

        private void Clean() {
            _Dirty = false;
            if (CheckEnumKeyIntact() == false) UpdateEnumKey();
        }
    }
    #endif
}
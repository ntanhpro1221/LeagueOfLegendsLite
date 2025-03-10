using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NGDtuanh.PropertySet {
    /// <summary>
    /// Set of value with the same type. Each element can be access easier through enum key
    /// </summary>
    /// <typeparam name="TKey">Enum type of key</typeparam>
    /// <typeparam name="TValue">Data type of each element</typeparam>
    [Serializable]
    public class PropertySet<TKey, TValue> :
        ISerializationCallbackReceiver
      , IReadOnlyCollection<KeyValuePair<TKey, TValue>>
        where TKey : Enum {
        private TKey[]                _Keys;
        private Dictionary<TKey, int> _HashedKeys;

        public int      Count  => _Keys.Length;
        public TKey[]   Keys   => _Keys.ToArray();
        public TValue[] Values => _Values.ToArray();

        #region SERIALIZED FIELD

        [SerializeField] private TValue[] _Values;

        #if UNITY_EDITOR
        #pragma warning disable CS0414
        [SerializeField] private bool     _Dirty;
        [SerializeField] private string[] _PrevKeyNames;
        [SerializeField] private int[]    _PrevKeyValues;
        [SerializeField] private bool[]   _IsVisible;
        [SerializeField] private int      _VisibleCount;
        [SerializeField] private string   _SearchText;
        #pragma warning restore CS0414
        #endif

        #endregion

        #if UNITY_EDITOR
        private Utils.Editor.ScriptReloadDetector _ScriptReloadDetector;
        #endif

        public PropertySet() {
            CorrectKeys();
            _Values = new TValue[_Keys.Length];

            #if UNITY_EDITOR
            _PrevKeyNames  = new string[_Keys.Length];
            _PrevKeyValues = new int[_Keys.Length];
            _IsVisible     = new bool[_Keys.Length];
            #endif
        }

        public PropertySet(ICollection<KeyValuePair<TKey, TValue>> source) {
            CorrectKeys();
            _Values = new TValue[_Keys.Length];

            Dictionary<TKey, TValue> dictSource  = new(source);
            foreach (var key in _Keys) this[key] = dictSource[key];
        }

        private void CorrectKeys() {
            _Keys = (TKey[])Enum.GetValues(typeof(TKey));
            _HashedKeys = Enumerable
                .Range(0, _Keys.Length)
                .ToDictionary(id => _Keys[id], id => id);
        }

        public TValue this[TKey key] {
            get => _Values[_HashedKeys[key]];
            set => _Values[_HashedKeys[key]] = value;
        }

        public void OnBeforeSerialize() {
            #if UNITY_EDITOR
            if (!_ScriptReloadDetector.IsReloaded_Update()) return;
            _Dirty = true;
            #endif

            CorrectKeys();
        }

        public void OnAfterDeserialize() { }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new(_Keys[i], _Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public static explicit operator Dictionary<TKey, TValue>(PropertySet<TKey, TValue> value)
            => new(value);

        public static explicit operator PropertySet<TKey, TValue>(Dictionary<TKey, TValue> value)
            => new(value);
    }
}
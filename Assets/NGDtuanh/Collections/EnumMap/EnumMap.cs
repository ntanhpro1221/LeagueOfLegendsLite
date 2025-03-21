using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace NGDtuanh.Collections {
    [Serializable]
    public class EnumMap<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>
      , ISerializationCallbackReceiver
        where TKey : struct, Enum {
        [SerializeField] internal TKey[]                _Keys;
        [SerializeField] internal WrapperBase<TValue>[] _Values;
        #if UNITY_EDITOR
        [SerializeField] internal string[] _KeyNames;
        [SerializeField] internal Hash128  _EditorSessionCode;
        [SerializeField] internal bool     _KeySynced;
        #endif

        private Dictionary<TKey, int> _HashedKeys = new();

        public int                 Count  => _Keys.Length;
        public IEnumerable<TKey>   Keys   => Array.AsReadOnly(_Keys);
        public IEnumerable<TValue> Values => Array.AsReadOnly(_Values.Cast<TValue>().ToArray());

        public EnumMap() {
            _Keys   = (TKey[])Enum.GetValues(typeof(TKey));
            _Values = new WrapperBase<TValue>[_Keys.Length];

            #if UNITY_EDITOR
            _KeyNames = new string[_Keys.Length];
            for (int i = 0; i < _Keys.Length; ++i)
                _KeyNames[i] = _Keys[i].ToString();
            #endif

            ReHashKeys();
        }
        
        public EnumMap(IReadOnlyCollection<KeyValuePair<TKey, TValue>> source) : this() {
            foreach (var (key, value) in source) {
                if (!_HashedKeys.ContainsKey(key)) throw new InvalidKeyException();
                this[key] = value;
            }
        }

        public Dictionary<TKey, TValue> ToDictionary() {
            var dict = new Dictionary<TKey, TValue>();
            for (int i = 0; i < Count; ++i) dict.Add(_Keys[i], _Values[i]);
            return dict;
        }

        public bool ContainsKey(TKey key) => true; // Yes, of course.

        public bool TryGetValue(TKey key, out TValue value) {
            try {
                value = this[key];
                return true;
            }
            catch {
                value = default;
                return false;
            }
        }

        public TValue this[TKey key] {
            get => _Values[_HashedKeys[key]];
            set {
                _Values[_HashedKeys[key]]       ??= new(value);
                _Values[_HashedKeys[key]].Value =   value;
            }
        }

        private void ReHashKeys() {
            _HashedKeys.Clear();
            for (int i = 0; i < _Keys.Length; ++i)
                _HashedKeys.Add(_Keys[i], i);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnBeforeSerialize() { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnAfterDeserialize() {
            #if UNITY_EDITOR
            if (!_KeySynced || _HashedKeys.Count != _Keys.Length) ReHashKeys();
            _KeySynced = true;
            #else
            if (_HashedKeys.Count != _Keys.Length) ReHashKeys();
            #endif
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new(_Keys[i], _Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class InvalidKeyException : Exception {
            public InvalidKeyException() : base("Given key is not correspond to true enum key") { }
        }
    }
}
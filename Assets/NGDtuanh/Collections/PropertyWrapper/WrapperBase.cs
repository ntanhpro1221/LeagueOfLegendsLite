using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NGDtuanh.Collections {
    [Serializable]
    public class WrapperBase<TValue> : IEquatable<TValue> {
        public const string ValueSerializeName = "_Value";

        // ReSharper disable once Unity.RedundantFormerlySerializedAsAttribute
        [SerializeField] [FormerlySerializedAs(ValueSerializeName)]
        protected TValue _Value;

        public virtual TValue Value {
            get => _Value;
            set => _Value = value;
        }

        public WrapperBase() { }
        public WrapperBase(in TValue value) => _Value = value;

        public          bool   Equals(TValue other) => _Value == null ? other == null : _Value.Equals(other);
        public override string ToString()           => _Value.ToString();

        public static implicit operator TValue(WrapperBase<TValue> value)
            => value == null ? default : value.Value;
    }
}
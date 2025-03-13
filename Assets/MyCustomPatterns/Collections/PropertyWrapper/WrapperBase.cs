using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NGDtuanh.Collections.PropertyWrapper {
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

        public          bool   Equals(TValue other) => Value.Equals(other);
        public override string ToString()           => Value.ToString();

        public static implicit operator TValue(WrapperBase<TValue> value) => value.Value;
    }
}
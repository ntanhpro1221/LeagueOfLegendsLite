using System;
using UnityEngine;
using UnityEngine.Events;

namespace NGDtuanh.Collections.PropertyWrapper {
    /// <summary>
    /// Invoke your action when its value is changed
    /// </summary>
    [Serializable]
    public class Bindable<TValue> : WrapperBase<TValue> {
        [SerializeField] private UnityEvent<TValue> _OnChanged = new();

        public override TValue Value {
            get => _Value;
            set {
                if (!Equals(value)) _OnChanged.Invoke(_Value = value);
            }
        }

        public Bindable(in TValue value) => _Value = value;
        public static implicit operator Bindable<TValue>(TValue value) => new(value);

        public void AddListener(UnityAction<TValue>    callback) => _OnChanged.AddListener(callback);
        public void RemoveListener(UnityAction<TValue> callback) => _OnChanged.RemoveListener(callback);
    }
}

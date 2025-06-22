using System;
using UnityEngine;
using UnityEngine.Events;

namespace NGDtuanh.Collections {
    /// <summary>
    /// Invoke your action when its value is changed
    /// </summary>
    [Serializable]
    public class Bindable<TValue> : WrapperBase<TValue> {
        public delegate void OnValueChangedDel(in TValue oldVal, in TValue newVal);

        public event OnValueChangedDel OnBeforeChanged;

        public override TValue Value {
            set => ChangeValue(value);
        }

        /// <returns>True when value is changed.</returns>
        public bool ChangeValue(in TValue newVal) {
            if (Equals(newVal)) return false;

            ForceAssignAndUpdate(newVal);
            return true;
        }

        public Bindable() { }
        public Bindable(in TValue         value) : base(value) { }
        public Bindable(OnValueChangedDel onChanged) => OnBeforeChanged += onChanged;

        public static implicit operator Bindable<TValue>(TValue value) => new(value);

        public void ForceAssignAndUpdate(in TValue newValue) {
            OnBeforeChanged?.Invoke(oldVal: _Value, newVal: newValue);
            _Value = newValue;
        }
    }
}
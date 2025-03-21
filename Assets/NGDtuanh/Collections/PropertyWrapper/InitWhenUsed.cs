using System;

namespace NGDtuanh.Collections {
    [Serializable]
    public class InitWhenUsed<TValue> : WrapperBase<TValue> {
        private Func<TValue> _Source;
        private bool         _NeedInit;

        public override TValue Value {
            get {
                if (_NeedInit) {
                    if (_Source != null) _Value = _Source.Invoke();
                    _NeedInit = !_NeedInit;
                }

                return _Value;
            }
            set => _Value = value;
        }

        public InitWhenUsed(Func<TValue> source) : base(default) {
            _Source   = source;
            _NeedInit = true;
        }

        private InitWhenUsed(TValue value) : base(value) {
            _NeedInit = false;
        }

        public static implicit operator InitWhenUsed<TValue>(TValue value) => new(value);
    }
}
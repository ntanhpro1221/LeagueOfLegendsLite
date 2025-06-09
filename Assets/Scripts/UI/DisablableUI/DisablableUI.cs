using UnityEngine;

public abstract class DisablableUI<TTarget, TProp> :
    MonoBehaviour
  , IDisablableUI
    where TTarget : MonoBehaviour {
    private TTarget _Target;

    protected TTarget Target {
        get {
            if (_Target == null) {
                _Target = GetComponent<TTarget>();
            }

            return _Target;
        }
    }

    [SerializeField] private TProp _Enable;
    [SerializeField] private TProp _Disable;

    protected abstract TProp PropSetter { set; }

    private void Awake() {
        GetComponentInParent<DisablableUIRoot>(includeInactive: true).Register(
            ((IDisablableUI)this).OnEnable
          , ((IDisablableUI)this).OnDisable);
    }

    void IDisablableUI.OnEnable() =>
        PropSetter = _Enable;

    void IDisablableUI.OnDisable() =>
        PropSetter = _Disable;
}
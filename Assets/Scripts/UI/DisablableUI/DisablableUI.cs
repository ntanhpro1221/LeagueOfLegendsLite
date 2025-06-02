using UnityEngine;

public abstract class DisablableUI<TTarget, TProp> :
    MonoBehaviour
  , IDisablableUI
    where TTarget : MonoBehaviour {
    protected TTarget _Target;

    [SerializeField] private TProp _Enable;
    [SerializeField] private TProp _Disable;

    protected abstract TProp PropSetter { set; }

    private void Awake() {
        GetComponentInParent<DisablableUIRoot>(includeInactive: true).Register(
            ((IDisablableUI)this).OnEnable
          , ((IDisablableUI)this).OnDisable);

        _Target = GetComponent<TTarget>();
    }

    void IDisablableUI.OnEnable() =>
        PropSetter = _Enable;

    void IDisablableUI.OnDisable() =>
        PropSetter = _Disable;
}
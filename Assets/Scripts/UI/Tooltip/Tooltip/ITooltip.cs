using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public abstract class ITooltip<TWindow> :
    MonoBehaviour
  , IPointerEnterHandler
  , IPointerExitHandler
    where TWindow : ITooltipWindow {
    private bool    _HaveWindow;
    private TWindow _Window;

    public TWindow Window {
        get {
            if (!_HaveWindow) {
                _HaveWindow = true;
                _Window     = TooltipWindowHolder.Instance.GetWindow<TWindow>();
                _Window.gameObject.SetActive(false);
            }

            return _Window;
        }
    }

    private RectTransform  _WindowTrans;
    private Vector2Control _MousePos;
    private bool           _IsHovering;
    private bool           _IsShowing;
    private float          _CurThreshold;

    [Header("THRESHOLD")]
    [SerializeField] private float _ShowThreshold;

    [SerializeField] private float _HideThreshold;

    [Header("WINDOW ANCHOR")]
    [SerializeField] private AnchorType _AnchorType;

    [Tooltip("Dynamic: offset with mouse | Static: offset with root")]
    [SerializeField] private Vector2 _Offset;

    private void Awake() {
        _WindowTrans = (RectTransform)Window.transform;
        _MousePos    = Mouse.current.position;

        if (_AnchorType == AnchorType.Static) _WindowTrans.SetParent(transform);
        _WindowTrans.anchoredPosition = _Offset;
    }

    private void Update() {
        if (_IsHovering != _IsShowing) {
            _CurThreshold -= Time.deltaTime;
            if (_CurThreshold <= 0) {
                ResetThreshold();
                _IsShowing.Flip();
                Window.gameObject.SetActive(_IsShowing);
            }
        } else ResetThreshold();

        if (_IsShowing && _AnchorType == AnchorType.FollowMouse) {
            _WindowTrans.anchoredPosition -= _Offset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _WindowTrans, _MousePos.value, null, out var targetPos);
            _WindowTrans.anchoredPosition += targetPos + _Offset;
        }
    }

    public void ResetThreshold() {
        _CurThreshold = _IsShowing
            ? _HideThreshold
            : _ShowThreshold;
    }

    private void OnDisable() {
        // Force hide window 
        Window.gameObject.SetActive(_IsHovering = _IsShowing = false);
        ResetThreshold();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        _IsHovering = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        _IsHovering = false;
    }

    public enum AnchorType {
        FollowMouse
      , Static
    }
}
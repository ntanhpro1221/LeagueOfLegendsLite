using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public abstract class ITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    protected ITooltipWindow Window { get; private set; }
    private   RectTransform  _WindowTrans;
    private   Vector2Control _MousePos;
    private   bool           _IsHovering;
    private   bool           _IsShowing;
    private   float          _CurThreshold;
    private   Camera         _Cam;
    
    [Header("THRESHOLD")]
    [SerializeField] private float _ShowThreshold;
    [SerializeField] private float _HideThreshold;

    [Header("WINDOW ANCHOR")]
    [SerializeField] private AnchorType _AnchorType;
    [SerializeField] private Vector2    _MouseOffset;

    private void Awake() {
        Window       = GetComponentInChildren<ITooltipWindow>(true);
        _WindowTrans = Window.transform as RectTransform;
        _MousePos    = Mouse.current.position;
        _Cam         = Camera.main;
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
            _WindowTrans.anchoredPosition -= _MouseOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _WindowTrans, _MousePos.value, null, out var targetPos);
            _WindowTrans.anchoredPosition += targetPos + _MouseOffset;
        }
    }

    public void ResetThreshold() {
        _CurThreshold = _IsShowing
            ? _HideThreshold
            : _ShowThreshold;
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
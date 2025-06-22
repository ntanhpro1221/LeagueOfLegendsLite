using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(
    typeof(CanvasGroup)
  , typeof(IDragItem))]
[DisallowMultipleComponent]
public class Dragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private CanvasGroup    _CanvasGroup;
    public  IDragItem      Item     { get; private set; }
    public  Type           ItemType { get; private set; }
    private Transform      _Trans;
    private Vector2Control _MousePos;

    private Transform _DragParent;
    private Transform _NormalParent;
    private Vector3   _NormalLocPos;

    private void Awake() {
        _CanvasGroup = GetComponent<CanvasGroup>();
        ItemType     = (Item = GetComponent<IDragItem>()).GetType();
        _Trans       = transform;
        _MousePos    = Mouse.current.position;

        _DragParent   = CanvasInspector.Instance.RectTrans;
        _NormalParent = _Trans.parent;
        _NormalLocPos = _Trans.localPosition;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        _CanvasGroup.blocksRaycasts = false;
        _Trans.SetParent(_DragParent);
        DragDropCenter.PostBeginDragItem(ItemType, this);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        _Trans.position = _MousePos.value;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        _CanvasGroup.blocksRaycasts = true;
        _Trans.SetParent(_NormalParent);
        DragDropCenter.PostEndDragItem(ItemType, this);
        _Trans.localPosition = _NormalLocPos;
    }
}
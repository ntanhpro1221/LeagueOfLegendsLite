using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(IFilteredDropHandler))]
[DisallowMultipleComponent]
public class Dropper : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    private IFilteredDropHandler _DropHandler;

    private void Awake() {
        DragDropCenter.RegisterDropHandler(_DropHandler = GetComponent<IFilteredDropHandler>());
    }

    private bool TryGetTargetItem(PointerEventData eventData, out Dragger dragger) {
        dragger = null;

        var draggerObj = eventData.pointerDrag;

        // Not dragging item
        if (draggerObj == null) return false;

        dragger = draggerObj.GetComponent<Dragger>();

        // Not exit draggable item
        if (dragger == null) return false;

        // Item not valid
        if (_DropHandler.TargetItem != dragger.ItemType) return false;

        return true;
    }

    public void OnDrop(PointerEventData eventData) {
        if (TryGetTargetItem(eventData, out var item)) _DropHandler.OnItemDrop(item);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (TryGetTargetItem(eventData, out var item)) _DropHandler.OnItemEnter(item);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (TryGetTargetItem(eventData, out var item)) _DropHandler.OnItemExit(item);
    }
}
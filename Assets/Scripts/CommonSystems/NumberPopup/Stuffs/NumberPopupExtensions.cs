using UnityEngine;

public static class NumberPopupExtensions {
    public static void Popup(this NumberPopup.Id id, int value, Transform parent) {
        var item = NumberPopupPool.Instance.GetItem(id);
        item.transform.SetParent(parent);
        item.Setup(id, value);
    }
}
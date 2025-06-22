using NGDtuanh.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RecipeNode : MonoBehaviour {
    [SerializeField] private Transform     _ChildHolder;
    [SerializeField] private RectTransform _HorPathImage;
    [SerializeField] private GameObject    _VerPathDownImage;
    [SerializeField] private ItemUIShop    _ItemUI;

    public ItemUIShop UI => _ItemUI;

    public void UpdatePathToChild() {
        int childCount = _ChildHolder.childCount;
        _VerPathDownImage.SetActive(childCount > 0);

        // Only need hor path when there are more than one child.
        bool useHorPath = childCount > 1;
        _HorPathImage.gameObject.SetActive(useHorPath);
        if (useHorPath) { // Update hor line rect
            float myWidth         = ((RectTransform)transform).rect.width;
            float firstChildWidth = ((RectTransform)_ChildHolder.GetChild(0).transform).rect.width;
            float lastChildWidth  = ((RectTransform)_ChildHolder.GetChild(childCount - 1).transform).rect.width;

            _HorPathImage.anchorMin = new Vector2((firstChildWidth / 2)          / myWidth, 0);
            _HorPathImage.anchorMax = new Vector2((myWidth - lastChildWidth / 2) / myWidth, 1);
        }
    }

    public void SetNodeRecursive(ItemId itemId, RecipeVisualizer visualizer) {
        var allItem  = GameSO.Item;
        var thisItem = allItem[itemId];
        _ItemUI.InitAll(itemId, thisItem);
        foreach (var childId in thisItem.recipe)
            visualizer
                .GetNodeUIFor(_ChildHolder.transform)
                .SetNodeRecursive(childId, visualizer);
    }

    public void UpdateStateRecursive(
        ref Strum.SlotItem.Fields<bool>         used
      , in  EnumMap<SlotItemId, IItemUIWrapper> itemUIs
      , in  Strum.Items.Fields<bool>            buyable
      , bool                                    purchased = false) {
        for (var slot = Strum.SlotItem.First_Item
             ; !purchased && slot <= Strum.SlotItem.Last_Item
             ; ++slot) {
            if (used[slot]) continue;

            var itemUI = (ItemUI)itemUIs[slot];
            if (!itemUI.Core.gameObject.activeSelf) continue;

            if (_ItemUI.CurItem != itemUI.CurItem) continue;

            purchased = used[slot] = true;
        }

        _ItemUI.UpdateState(buyable[_ItemUI.CurItem], purchased);

        foreach (Transform child in _ChildHolder)
            child.GetComponent<RecipeNode>().UpdateStateRecursive(ref used, itemUIs, buyable, purchased);
    }

    public void UpdateCost(in EnumMap<SlotItemId, IItemUIWrapper> itemUIs) {
        var myItem  = _ItemUI.CurItem;
        var allItem = GameSO.Item;

        Strum.SlotItem.Fields<bool> used = default;
        ModifyItemSystem.FindSacrificeSlots(ref used, myItem, itemUIs);

        var requiredGold = allItem[myItem].settings.cost;
        for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot)
            if (used[slot])
                requiredGold -= allItem[((ItemUI)itemUIs[slot]).CurItem].settings.cost;

        _ItemUI.SetCost(requiredGold);
    }

    #region FOR EDITOR TESTING

    #if UNITY_EDITOR
    private void OnEnable() {
        if (Application.isPlaying) return;

        var visualizer = FindFirstObjectByType<RecipeVisualizer>();
        if (visualizer == null) {
            Debug.LogWarning("NGDtuanh recipe visualizer is null");
            return;
        }

        visualizer.UsedNodeUI.Add(this);
        visualizer.DelayedUpdateAllPathToChild();
    }

    private void OnDisable() {
        if (Application.isPlaying) return;

        var visualizer = FindFirstObjectByType<RecipeVisualizer>();
        if (visualizer == null) {
            Debug.LogWarning("NGDtuanh recipe visualizer is null");
            return;
        }

        visualizer.UsedNodeUI.Remove(this);
        visualizer.DelayedUpdateAllPathToChild();
    }
    #endif

    #endregion
}
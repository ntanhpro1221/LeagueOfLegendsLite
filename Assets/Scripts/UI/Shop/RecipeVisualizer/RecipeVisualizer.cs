using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RecipeVisualizer : MonoBehaviour {
    [SerializeField] private RecipeNode _RootItem;
    [SerializeField] private RecipeNode _NodePrefab;

    #region NODE POOL

    private readonly Stack<RecipeNode>   _AvailableNodeUI = new();
    public readonly  HashSet<RecipeNode> UsedNodeUI       = new();

    public RecipeNode GetNodeUIFor(Transform trans) {
        if (_AvailableNodeUI.Count == 0)
            _AvailableNodeUI.Push(Instantiate(_NodePrefab, transform));

        var node = _AvailableNodeUI.Pop();
        node.gameObject.SetActive(true);
        UsedNodeUI.Add(node);
        node.transform.SetParent(trans);
        return node;
    }

    private void ReleaseAllNodeUI() {
        foreach (var node in UsedNodeUI) {
            node.gameObject.SetActive(false);
            node.transform.SetParent(transform);
            _AvailableNodeUI.Push(node);
        }

        UsedNodeUI.Clear();
    }

    #endregion

    private void Awake() {
        LazyObserver_Battle.AddListener(LazyObserver_Battle.Events.SlotChanged, UpdatePurchasedItemAndCost);
        ShopUI.Instance.Buyable.OnBeforeChanged += UpdateBuyable;
    }

    private void UpdatePurchasedItemAndCost() {
        var inspector = ShopUI.Instance.Inspector;
        if (inspector.SelectedItem == null) return;

        var itemUIs = PlayerHUD.Instance.ActivableItems.Items;

        // Update purchased state
        Strum.SlotItem.Fields<bool> used = default;
        _RootItem.UpdateStateRecursive(ref used, itemUIs, ShopUI.Instance.Buyable.Value);

        // Update cost
        _RootItem.UpdateCost(itemUIs);
        foreach (var node in UsedNodeUI) node.UpdateCost(itemUIs);
    }

    private void UpdateBuyable(in Strum.Items.Fields<bool> _, in Strum.Items.Fields<bool> buyable) {
        var inspector = ShopUI.Instance.Inspector;
        if (inspector.SelectedItem == null) return;

        _RootItem.UI.UpdateState(buyable[_RootItem.UI.CurItem]);
        foreach (var node in UsedNodeUI.Where(node => !node.UI.Purchased))
            node.UI.UpdateState(buyable[node.UI.CurItem]);
    }

    #region UPDATE PATH TO CHILD

    private void UpdateAllPathToChild() {
        if (_RootItem == null) {
            Debug.LogWarning($"NGDtuanh recipe tree: root item is null. If this only happen when you load scene, this is normal.");
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_RootItem.transform);

        // Root item is only added to UsedNode in editor, so we still leave it updated separately here.
        _RootItem.UpdatePathToChild();
        foreach (var node in UsedNodeUI) node.UpdatePathToChild();
    }

    private IEnumerator UpdateAllPathToChildCoroutine() {
        yield return null;

        UpdateAllPathToChild();
    }

    public void DelayedUpdateAllPathToChild() {
        // Use delayCall on editor instead of coroutine.
        #if UNITY_EDITOR
        if (!Application.isPlaying) {
            UnityEditor.EditorApplication.delayCall += UpdateAllPathToChild;
            return;
        }
        #endif

        StartCoroutine(UpdateAllPathToChildCoroutine());
    }

    #endregion

    public void ShowRecipeFor(ItemId itemId) {
        ReleaseAllNodeUI();
        _RootItem.SetNodeRecursive(itemId, this);
        UpdatePurchasedItemAndCost();

        DelayedUpdateAllPathToChild();
    }
}
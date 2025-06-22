using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEngine;

public class IndicatorShower : MonoBehaviour {
    [Header("CONCRETES")]
    [SerializeField] private IndicatorConcreteBase _IndicatorNormalAttack;

    [SerializeField] private EnumMap<SlotItemId, IndicatorConcreteBase> _IndicatorConcretes;

    [field: Space]
    [Header("COMPONENTS")]
    [field: SerializeField] public MeshRenderer NormalAttack { get; private set; }

    [field: SerializeField] public MeshRenderer StickyIndicator      { get; private set; }
    [field: SerializeField] public MeshRenderer DynamicIndicator     { get; private set; }
    [field: SerializeField] public MeshRenderer SplitStickyIndicator { get; private set; }

    public List<MeshRenderer> MultiIndicator { get; private set; } = new();

    [field: SerializeField] public Transform MultiIndicatorRoot { get; private set; }

    private IndicatorConcreteBase _CurConcrete;
    private ActivableItemData     _NullItemData;
    private bool                  _ShowNormalAttack;

    public void EnsureMultiIndicatorSize(int requiredSize) {
        while (requiredSize > MultiIndicator.Count)
            MultiIndicator.Add(IndicatorProvider.Instance.SpawnNewIndicator(MultiIndicatorRoot));
    }

    public void UpdateIndicatorAt(SlotItemId slot, IndicatorConcreteBase indicator) =>
        _IndicatorConcretes[slot] = indicator;
    
    public void UpdateShower(in Metadata metadata) =>
        UpdateShower(metadata, ref _NullItemData);

    public void UpdateShower(
        in  Metadata          metadata
      , ref ActivableItemData itemData) {
        var newConcrete = metadata.IsWithoutItem() ? null : _IndicatorConcretes[metadata.itemKey];

        if (newConcrete != _CurConcrete) {
            if (_CurConcrete != null) _CurConcrete.Disable(this);
            _CurConcrete = newConcrete;
            if (_CurConcrete != null) _CurConcrete.Enable(this);
        }

        if (metadata.showNormalAttack != _ShowNormalAttack) {
            _ShowNormalAttack = metadata.showNormalAttack;
            if (_ShowNormalAttack)
                _IndicatorNormalAttack.Enable(this);
            else _IndicatorNormalAttack.Disable(this);
        }

        if (_CurConcrete != null)
            _CurConcrete.UpdateShower(this, metadata, ref itemData);
        if (_ShowNormalAttack)
            _IndicatorNormalAttack.UpdateShower(this, metadata, ref itemData);
    }

    public struct Metadata {
        public SlotItemId itemKey;

        public int ownerLevel;
        public int selfLevel;

        // For normal attack
        public bool     showNormalAttack;
        public float_Q3 attackRange;

        // For turret
        public float3_Q3 ownChampPos;
        public bool      ownChampIsTarget;

        public InputForActivableItemData input;
        public ItemActiveCondition       condition;
    }
}
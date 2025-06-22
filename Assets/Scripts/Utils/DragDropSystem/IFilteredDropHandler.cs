using System;

public interface IFilteredDropHandler {
    Type TargetItem { get; }

    void OnItemDrop(Dragger dragger);

    void OnItemEnter(Dragger dragger);
    void OnItemExit(Dragger  dragger);

    void OnItemBeginDrag(Dragger dragger);
    void OnItemEndDrag(Dragger   dragger);
}

public interface IFilteredDropHandler<TTarget> : IFilteredDropHandler where TTarget : class, IDragItem {
    static readonly Type      _TargetItem = typeof(TTarget);
    Type IFilteredDropHandler.TargetItem => _TargetItem;

    void IFilteredDropHandler.OnItemDrop(Dragger dragger) => OnItemDrop(dragger.Item as TTarget);

    void IFilteredDropHandler.OnItemEnter(Dragger dragger) => OnItemEnter(dragger.Item as TTarget);
    void IFilteredDropHandler.OnItemExit(Dragger  dragger) => OnItemExit(dragger.Item as TTarget);

    void IFilteredDropHandler.OnItemBeginDrag(Dragger dragger) => OnItemBeginDrag(dragger.Item as TTarget);
    void IFilteredDropHandler.OnItemEndDrag(Dragger   dragger) => OnItemEndDrag(dragger.Item as TTarget);

    void OnItemDrop(TTarget item);

    void OnItemEnter(TTarget item);
    void OnItemExit(TTarget  item);

    void OnItemBeginDrag(TTarget item);
    void OnItemEndDrag(TTarget   item);
}
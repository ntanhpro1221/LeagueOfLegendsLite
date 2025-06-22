using System;
using System.Collections.Generic;
using NGDtuanh.Singleton;
using UnityEngine;

public class DragDropCenter : SceneSingleton<DragDropCenter> {
    private readonly Dictionary<Type, List<IFilteredDropHandler>> _DropHandlerDict = new();

    private List<IFilteredDropHandler> GetOrAddDropHandlers(Type type) {
        if (!_DropHandlerDict.ContainsKey(type)) _DropHandlerDict.Add(type, new List<IFilteredDropHandler>());

        return _DropHandlerDict[type];
    }

    public static void RegisterDropHandler(IFilteredDropHandler dropHandler) =>
        Instance.GetOrAddDropHandlers(dropHandler.TargetItem).Add(dropHandler);

    public static void PostBeginDragItem(Type itemType, Dragger dragger) {
        foreach (var dropHandler in Instance.GetOrAddDropHandlers(itemType)) dropHandler.OnItemBeginDrag(dragger);
    }

    public static void PostEndDragItem(Type itemType, Dragger dragger) {
        foreach (var dropHandler in Instance.GetOrAddDropHandlers(itemType)) dropHandler.OnItemEndDrag(dragger);
    }
}
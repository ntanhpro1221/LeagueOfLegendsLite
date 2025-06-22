using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.Singleton;

public abstract class ILazyObserver<TEvents> :
    SceneSingleton<ILazyObserver<TEvents>>
    where TEvents : struct, Enum {
    private readonly Dictionary<TEvents, Action> _Listeners = ((TEvents[])Enum.GetValues(typeof(TEvents))).ToDictionary(
        keySelector: item => item
      , elementSelector: _ => default(Action));

    public static void AddListener(TEvents    id, Action callback) => Instance._Listeners[id] += callback;
    public static void RemoveListener(TEvents id, Action callback) => Instance._Listeners[id] -= callback;
    public static void PostEvent(TEvents      id) => Instance._Listeners[id]?.Invoke();
}
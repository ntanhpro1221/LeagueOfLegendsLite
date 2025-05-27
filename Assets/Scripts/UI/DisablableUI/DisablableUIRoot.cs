using System;
using UnityEngine;

public class DisablableUIRoot : MonoBehaviour {
    private event Action _OnEnable;
    private event Action _OnDisable;

    public void Register(Action onEnable, Action onDisable) {
        _OnEnable  += onEnable;
        _OnDisable += onDisable;
    }

    public void EnableAll()  => _OnEnable?.Invoke();
    public void DisableAll() => _OnDisable?.Invoke();
}
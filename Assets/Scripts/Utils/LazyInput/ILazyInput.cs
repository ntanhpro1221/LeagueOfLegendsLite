using System;
using NGDtuanh.Singleton;
using UnityEngine.InputSystem;

public abstract class ILazyInput<TInputAction> :
    SceneSingleton<ILazyInput<TInputAction>>
    where TInputAction : class, IInputActionCollection2, IDisposable, new() {
    private       TInputAction _Input;
    public static TInputAction Input => Instance._Input;

    protected override void OnTouched() {
        base.OnTouched();

        _Input ??= new TInputAction();
    }

    protected override void Awake() {
        base.Awake();

        _Input ??= new TInputAction();
    }

    private void OnEnable() {
        _Input.Enable();
    }

    private void OnDisable() {
        _Input.Disable();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        
        _Input.Dispose();
    }
}
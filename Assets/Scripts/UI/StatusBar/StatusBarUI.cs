using System.Collections.Generic;
using NGDtuanh.Singleton;
using TMPro;
using UnityEngine;

public class StatusBarUI : SceneSingleton<StatusBarUI> {
    [Header("TEXT ELEMENTS")]
    [SerializeField] private TextMeshProUGUI _FPS;

    [SerializeField] private TextMeshProUGUI _GlobalKDA;
    [SerializeField] private TextMeshProUGUI _KDA;
    [SerializeField] private TextMeshProUGUI _CS;
    [SerializeField] private TextMeshProUGUI _Timer;
    [SerializeField] private TextMeshProUGUI _Ping;

    [Header("SETTINGS")]
    [Min(1)]
    [SerializeField] private int _FpsBufferSize;

    private Queue<float> _DeltaTimeBuffer = new();
    private float        _SumDeltaTime    = 0;

    private void UpdateFps() {
        _SumDeltaTime += Time.deltaTime;
        _DeltaTimeBuffer.Enqueue(Time.deltaTime);
        while (_DeltaTimeBuffer.Count > _FpsBufferSize)
            _SumDeltaTime -= _DeltaTimeBuffer.Dequeue();

        new TextUpdater.FPS { deltaTime = _SumDeltaTime / _DeltaTimeBuffer.Count }.Update(_FPS);
    }

    [Min(1)]
    [SerializeField] private int _PingBufferSize;

    private Queue<float> _RTTBuffer = new();
    private float        _SumRTT    = 0;

    private void UpdatePing(float rtt) {
        _SumRTT += rtt;
        _RTTBuffer.Enqueue(rtt);
        while (_RTTBuffer.Count > _PingBufferSize)
            _SumRTT -= _RTTBuffer.Dequeue();

        new TextUpdater.Ping { rtt = _SumRTT / _RTTBuffer.Count }.Update(_Ping);
    }

    private void Update() {
        UpdateFps();
    }

    public void ManualUpdateUI(
        in TextUpdater.GlobalKDA  globalKDA
      , in TextUpdater.KDA        kda
      , in TextUpdater.CreepScore creepScore
      , in TextUpdater.Timer      timer
      , in TextUpdater.Ping       ping) {
        globalKDA.Update(_GlobalKDA);
        kda.Update(_KDA);
        creepScore.Update(_CS);
        timer.Update(_Timer);
        UpdatePing(ping.rtt);
    }
}
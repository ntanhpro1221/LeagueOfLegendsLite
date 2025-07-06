using System;
using UnityEngine;

public class Alice : MonoBehaviour {
    private LanDiscoverer.Server _Server;
    private string               _CachedPlayerName;

    private void Awake() {
        var config = GameSO.RoomConnectionConfig;
        _Server = new LanDiscoverer.Server(
            config.Keyword.RoomBroadcast
          , config.BroadcastPort
          , config.BroadcastSleepTime
          , JsonUtility.ToJson(new RoomBroadcastData {
                PlayerName = _CachedPlayerName = HomeMenuUI.Instance.PlayerName
            }));
    }

    private void Update() {
        if (_CachedPlayerName != HomeMenuUI.Instance.PlayerName)
            _Server.UpdateData(JsonUtility.ToJson(new RoomBroadcastData {
                PlayerName = _CachedPlayerName = HomeMenuUI.Instance.PlayerName
            }));
    }

    private void OnApplicationQuit() {
        _Server?.Dispose();
        _Server = null;
    }
}
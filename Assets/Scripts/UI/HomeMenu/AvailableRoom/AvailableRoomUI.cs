using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailableRoomUI : MonoBehaviour {
    private LanDiscoverer.Client _Client;

    private readonly ConcurrentQueue<LanDiscoverer.ListenData> _DataPool = new();
    private readonly Dictionary<string, AvailableRoomUIItem>   _Rooms    = new();

    #region CONFIG SHORTCUT

    private RoomConnectionConfig _Config;

    private RoomConnectionConfig Config {
        get {
            if (_Config == null) _Config = GameSO.RoomConnectionConfig;

            return _Config;
        }
    }

    #endregion

    private void Awake() {
        _Client = new LanDiscoverer.Client(Config.Keyword.RoomBroadcast, Config.BroadcastPort, _DataPool);
    }

    private void Update() {
        // Handle host broadcast
        while (_DataPool.TryDequeue(out var data)) {
            var ip      = data.EndPoint.Address.ToString();
            var message = JsonUtility.FromJson<RoomBroadcastData>(data.Message);
            if (!_Rooms.ContainsKey(ip)) _Rooms.Add(ip, GetItemUI().Init(message.PlayerName, ip));

            _Rooms[ip].Beat(message.PlayerName);
        }

        // Remove expired room
        foreach (var ip in (
            from room in _Rooms
            where 1e3f * (Time.time - room.Value.LastBeatTime) > Config.BroadcastExpiredTime
            select room.Key).ToList()) {

            ReleaseItemUI(_Rooms[ip]);
            _Rooms.Remove(ip);
        }
    }

    private void OnDisable() {
        _Client?.Dispose();
        _Client = null;
    }

    private void OnApplicationQuit() {
        _Client?.Dispose();
        _Client = null;
    }

    #region POOL ITEM UI

    [SerializeField] private AvailableRoomUIItem _ItemPrefab;
    [SerializeField] private Transform           _ItemHolder;

    private readonly Stack<AvailableRoomUIItem> _AvailableItemUI = new();

    private AvailableRoomUIItem GetItemUI() {
        if (_AvailableItemUI.Count == 0)
            _AvailableItemUI.Push(Instantiate(_ItemPrefab, _ItemHolder));

        var result = _AvailableItemUI.Pop();
        result.gameObject.SetActive(true);
        return result;
    }

    private void ReleaseItemUI(AvailableRoomUIItem item) {
        item.gameObject.SetActive(false);
        _AvailableItemUI.Push(item);
    }

    #endregion
}
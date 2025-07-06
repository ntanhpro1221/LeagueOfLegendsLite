using TMPro;
using UnityEngine;

public class AvailableRoomUIItem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _RoomName;

    private string _IpAddress;

    public float LastBeatTime { get; private set; }

    public AvailableRoomUIItem Init(string roomName, string ipAddress) {
        _RoomName.text = "Room of: " + roomName;
        _IpAddress     = ipAddress;
        Beat(roomName);

        return this;
    }

    public void Beat(string roomName) => (LastBeatTime, _RoomName.text) = (Time.time, "Room of: " + roomName);

    public void OnJoinRoom() => HomeMenuUI.Instance.OnJoinRoom(_IpAddress);
}
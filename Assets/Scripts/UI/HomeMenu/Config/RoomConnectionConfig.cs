using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Room Connection Config", menuName = "Data/Room Connection Config")]
public class RoomConnectionConfig : ScriptableObject {
    public MessageKeyword Keyword;
    public ushort         BroadcastPort;
    public ushort         GamePort;

    [Tooltip("In millisecond")] public int BroadcastSleepTime;
    [Tooltip("In millisecond")] public int BroadcastExpiredTime;

    [Serializable]
    public class MessageKeyword {
        public string RoomBroadcast;
    }
}
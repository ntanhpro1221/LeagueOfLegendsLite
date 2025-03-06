using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionMenuUI : MonoBehaviour {
    [SerializeField] private LabeledDropdown _NetworkRole;
    [SerializeField] private LabeledInput    _IpAddress;
    [SerializeField] private LabeledInput    _Port;
    [SerializeField] private LabeledDropdown _TeamType;
    [SerializeField] private Button          _ConnectBtn;

    private Dictionary<int, NetworkRole> Id2NetworkRole = new();
    private Dictionary<int, TeamType>    Id2TeamType = new();
    
    private void Start() {
        InitMenu(); 
    }

    private void InitMenu() {
        // Network role dropdown
        var networkRoleValues = (NetworkRole[])Enum.GetValues(typeof(NetworkRole));
        for (int i = 0; i < networkRoleValues.Length; i++)
            Id2NetworkRole.Add(i, networkRoleValues[i]);
        _NetworkRole.WithDropdown(new() {
            itemList = networkRoleValues
                .Select(NetworkRole2Label)
                .ToList()
        });

        // Team type dropdown
        var teamTypeValues = (TeamType[])Enum.GetValues(typeof(TeamType));
        for (int i = 0; i < teamTypeValues.Length; i++)
            Id2TeamType.Add(i, teamTypeValues[i]);
        _TeamType.WithDropdown(new() {
            itemList = teamTypeValues
                .Select(TeamType2Label)
                .ToList()
        });

        // Connect button
        _ConnectBtn.onClick.AddListener(() => BattleConnectHelper.Connect(GetBattleConnectData()));
    }

    private static string NetworkRole2Label(NetworkRole role) => role switch {
        NetworkRole.Client => "Client"
      , NetworkRole.Server => "Server"
      , NetworkRole.Host   => "Host"
      , _                  => role.ToString()
    };

    private static string TeamType2Label(TeamType type) => type switch {
        TeamType.Blue      => "Blue"
      , TeamType.Red       => "Red"
      , TeamType.Spectator => "Spectator"
      , TeamType.DontCare  => "Dont care"
      , _                  => type.ToString()
    };

    public BattleConnectData GetBattleConnectData() => new() {
        networkRole = Id2NetworkRole[_NetworkRole.Dropdown.value]
      , endpoint    = NetworkEndpoint.Parse(_IpAddress.Input.text, ushort.Parse(_Port.Input.text))
      , teamType    = Id2TeamType[_TeamType.Dropdown.value]
    };
}
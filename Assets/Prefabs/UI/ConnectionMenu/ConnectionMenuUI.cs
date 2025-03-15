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
    [SerializeField] private LabeledDropdown _Champion;
    [SerializeField] private Button          _ConnectBtn;

    private readonly Dictionary<int, NetworkRole> Id2NetworkRole = new();
    private readonly Dictionary<int, TeamType>    Id2TeamType    = new();
    private readonly Dictionary<int, ChampionId>  Id2ChampionId  = new();

    private void Start() {
        InitMenu();
    }

    private void InitMenu() {
        // Network role dropdown
        InitDropdownItem(
            Id2NetworkRole
          , _NetworkRole
          , key => key switch {
                NetworkRole.Client => "Client"
              , NetworkRole.Server => "Server"
              , NetworkRole.Host   => "Host"
              , _                  => key.ToString()
            });

        // Team type dropdown
        InitDropdownItem(
            Id2TeamType
          , _TeamType
          , key => key switch {
                TeamType.Blue      => "Blue"
              , TeamType.Red       => "Red"
              , TeamType.Spectator => "Spectator"
              , TeamType.DontCare  => "Dont care"
              , _                  => key.ToString()
            });

        // champion dropdown
        InitDropdownItem(
            Id2ChampionId
          , _Champion
          , key => key switch {
                ChampionId.Ashe  => "Ashe"
              , ChampionId.Garen => "Garen"
              , ChampionId.Yasuo => "Yasuo"
              , _                => key.ToString()
            });

        // Connect button
        _ConnectBtn.onClick.AddListener(() => BattleConnectHelper.Connect(GetBattleConnectData()));
    }

    public BattleConnectData GetBattleConnectData() => new() {
        networkRole = Id2NetworkRole[_NetworkRole.Dropdown.value]
      , endpoint    = NetworkEndpoint.Parse(_IpAddress.Input.text, ushort.Parse(_Port.Input.text))
      , teamType    = Id2TeamType[_TeamType.Dropdown.value]
      , champion    = Id2ChampionId[_Champion.Dropdown.value]
    };
    
    private void InitDropdownItem<TEnum>(Dictionary<int, TEnum> id2Enum
                                       , LabeledDropdown        dropdown
                                       , Func<TEnum, string>    enum2Label) where TEnum : Enum {
        var values = (TEnum[])Enum.GetValues(typeof(TEnum));
        for (int i = 0; i < values.Length; i++)
            id2Enum.Add(i, values[i]);
        dropdown.WithDropdown(new() {
            itemList = values
                .Select(enum2Label)
                .ToList()
        });
    }
}
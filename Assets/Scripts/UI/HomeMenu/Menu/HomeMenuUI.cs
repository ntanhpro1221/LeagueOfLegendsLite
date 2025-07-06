using NGDtuanh.Singleton;
using UnityEngine;

public class HomeMenuUI : SceneSingleton<HomeMenuUI> {
    protected override void Awake() {
        base.Awake();

        Awake_PlayerName();
    }

    #region PLAYER NAME

    private const string DEFAULT_PLAYER_NAME = "Random Monkey";
    private const string KEY_PLAYER_NAME     = "player_name";

    [SerializeField] private LabeledInput PlayerNameInput;

    public string PlayerName => string.IsNullOrWhiteSpace(PlayerNameInput.Input.text) ? "Anonymous" : PlayerNameInput.Input.text;

    private void Awake_PlayerName() {
        PlayerNameInput.Input.text = PlayerPrefs.GetString(KEY_PLAYER_NAME, DEFAULT_PLAYER_NAME);
        PlayerNameInput.Input.onValueChanged.AddListener(newName => PlayerPrefs.SetString(KEY_PLAYER_NAME, newName));
    }

    #endregion

    #region CREATE AND JOIN ROOM

    public bool CreatingRoom { get; private set; }
    public bool JoiningRoom  { get; private set; }

    public void OnCreateRoom() {
        if (CreatingRoom || JoiningRoom) return;
        CreatingRoom = true;

        BanPickBootstrapper.Instance.StartBanPick_AsHost(PlayerName);
    }

    public void OnJoinRoom(string hostIp) {
        if (CreatingRoom || JoiningRoom) return;
        JoiningRoom = true;

        BanPickBootstrapper.Instance.StartBanPick_AsClient(PlayerName, hostIp);
    }

    #endregion
}
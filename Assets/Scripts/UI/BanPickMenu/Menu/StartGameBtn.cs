using System;
using NGDtuanh.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameBtn : MonoBehaviour {
    [SerializeField] private Button          _Button;
    [SerializeField] private TextMeshProUGUI _Text;

    private void Awake() {
        UpdateState(BanPickBootstrapper.Instance.IsHost, false);
    }

    public void UpdateState(bool isHost, bool isAllPlayerDonePickChamp) => _StateData[
        !isHost
            ? State.WaitingHostStartGame
            : isAllPlayerDonePickChamp
                ? State.CanStartGame
                : State.WaitingAllPlayerDonePickChamp
    ].ApplyTo(this);

    [SerializeField] private EnumMap<State, StateData> _StateData;

    public enum State {
        WaitingAllPlayerDonePickChamp
      , WaitingHostStartGame
      , CanStartGame
    }

    [Serializable]
    public class StateData {
        [SerializeField] private bool   BtnInteractable;
        [SerializeField] private string Text;

        public void ApplyTo(StartGameBtn target) {
            target._Button.interactable = BtnInteractable;
            target._Text.text           = Text;
        }
    }
}
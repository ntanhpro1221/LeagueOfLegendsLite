using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampMenuButtons : MonoBehaviour {
    [SerializeField] private Button _LockBtn;
    [SerializeField] private Button _SelectAnotherBtn;

    public void UpdateState(State state) {
        _LockBtn.gameObject.SetActive(state          == State.Selected);
        _SelectAnotherBtn.gameObject.SetActive(state == State.Locked);
    }

    public enum State {
        NotSelectedAnything
      , Selected
      , Locked
    }
}
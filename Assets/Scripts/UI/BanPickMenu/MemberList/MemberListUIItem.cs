using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemberListUIItem : MonoBehaviour {
    [SerializeField] private Image           _ChampAvatar;
    [SerializeField] private TextMeshProUGUI _Names;

    public void InitAll(in TeamMemberBuffer data) {
        if (data.lockedChamp) {
            var champData = GameSO.Champ[data.champ];
            _ChampAvatar.sprite = champData.avatar;
            _Names.text         = $"<b>{champData.name}</b>";
        } else {
            _ChampAvatar.sprite = null;
            _Names.text         = "<b>Selecting...</b>";
        }

        _Names.text += $"\n{data.playerName}";
    }
}
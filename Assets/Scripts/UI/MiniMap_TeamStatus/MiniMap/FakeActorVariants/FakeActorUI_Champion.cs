using UnityEngine;
using UnityEngine.UI;

public class FakeActorUI_Champion : FakeActorUI {
    [SerializeField] private DataSOReader _SOReader;
    
    [SerializeField] private Color        _AllyOutline;
    [SerializeField] private Color        _EnemyOutline;

    [SerializeField] private Image _Outline;

    public FakeActorUI_Champion Init(ChampionId id, bool isAlly) {
        _MainTex.sprite = _SOReader.Champ[id].avatar;
        _Outline.color  = isAlly ? _AllyOutline : _EnemyOutline;
        return this;
    }
}
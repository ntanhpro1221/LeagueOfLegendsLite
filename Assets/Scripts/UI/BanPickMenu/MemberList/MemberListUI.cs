using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class MemberListUI : MonoBehaviour {
    [SerializeField] private TeamType   _Team;
    [SerializeField] private GameObject _JoinBtn;

    public void ForceUpdateAllItemUI(in DynamicBuffer<TeamMemberBuffer> buffer, in NetworkId myNetId) {
        foreach (Transform oldItem in _ItemHolder)
            ReleaseItemUI(oldItem.GetComponent<MemberListUIItem>());

        bool imInThisTeam = false;

        foreach (var member in buffer)
            if (member.team == _Team) {
                GetItemUI().InitAll(member);
                if (member.netId.Value == myNetId.Value)
                    imInThisTeam = true;
            }

        _JoinBtn.SetActive(!imInThisTeam);
    }

    public void OnJoin() {
        _JoinBtn.SetActive(false);

        World.DefaultGameObjectInjectionWorld.EntityManager.SendRpc(new LockTeamRpc { teamId = _Team });
    }

    #region POOL ITEM UI

    [Header("---------ITEM POOL-------")]
    [SerializeField] private MemberListUIItem _ItemPrefab;

    [SerializeField] private Transform _ItemHolder;

    private readonly Stack<MemberListUIItem> _AvailableItemUI = new();

    private MemberListUIItem GetItemUI() {
        if (_AvailableItemUI.Count == 0)
            _AvailableItemUI.Push(Instantiate(_ItemPrefab, _ItemHolder));

        var result = _AvailableItemUI.Pop();
        result.gameObject.SetActive(true);
        result.transform.SetAsLastSibling();
        return result;
    }

    private void ReleaseItemUI(MemberListUIItem item) {
        item.gameObject.SetActive(false);
        item.transform.SetAsFirstSibling();
        _AvailableItemUI.Push(item);
    }

    #endregion
}
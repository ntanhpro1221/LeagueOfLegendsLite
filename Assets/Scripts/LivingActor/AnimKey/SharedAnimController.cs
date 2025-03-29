using UnityEngine;

public class SharedAnimController : MonoBehaviour {
    [SerializeField]
    private Animator _Animator;

    public SharedAnimKey CurAnim { get; private set; } = SharedAnimKey.Idle;

    public void SyncAnim(SharedAnimKey key) {
        if (CurAnim == key) return;

        _Animator.SetBool(CurAnim.StateVarName(), false);
        CurAnim = key;
        _Animator.SetBool(CurAnim.StateVarName(), true);
    }
}
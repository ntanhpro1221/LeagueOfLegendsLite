using UnityEngine;

public class SharedAnimController : MonoBehaviour {
    private Animator _Animator;

    public SharedAnimKey CurAnim { get; private set; } = SharedAnimKey.Idle;

    private void Awake() {
        _Animator = GetComponentInChildren<Animator>();
    }

    public void SyncAnim(SharedAnimKey key, ref bool isNeedRestart, bool hardCutAnim) {
        if (CurAnim != key) {
            _Animator.SetBool(CurAnim.StateVarName(), false);
            CurAnim = key;
            _Animator.SetBool(CurAnim.StateVarName(), true);

            if (hardCutAnim)
                _Animator.Play(CurAnim.KeyName());
        }

        if (isNeedRestart) {
            isNeedRestart = false;

            if (hardCutAnim)
                _Animator.Play(_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            else _Animator.CrossFade(_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0.2f, 0, 0);
        }
    }
}
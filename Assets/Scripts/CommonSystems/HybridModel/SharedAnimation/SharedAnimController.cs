using Unity.NetCode;
using UnityEngine;

public class SharedAnimController : MonoBehaviour {
    private Animator _Animator;
    private int      _CurrentSessionToRestart;

    public SharedAnimKey CurAnim { get; private set; } = SharedAnimKey.Idle;

    private void Awake() {
        (_Animator = GetComponentInChildren<Animator>())
            .gameObject.transform.localPosition = Vector3.zero;
    }

    public void SyncAnim(SharedAnimKey key, int newSession, bool hardCutAnim) {
        if (CurAnim != key) {
            _Animator.SetBool(CurAnim.StateVarName(), false);
            CurAnim = key;
            _Animator.SetBool(CurAnim.StateVarName(), true);

            if (hardCutAnim)
                _Animator.Play(CurAnim.KeyName());
        }

        if (_CurrentSessionToRestart != newSession) {
            _CurrentSessionToRestart = newSession;
            
            if (hardCutAnim)
                _Animator.Play(_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            else _Animator.CrossFade(_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0.2f, 0, 0);
        }
    }
}
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class RotationController : MonoBehaviour {
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> tween;

    private floatXZ_Q3 curTargetDir;
    
    private void Awake() {
        tween = transform
            .DORotate(Vector3.forward, 1)
            .SetEase(Ease.Linear)
            .SetAutoKill(false)
            .Pause();
    }

    private void OnDestroy() {
        tween.Kill();
    }

    public void RotateTo(floatXZ_Q3 dir) {
        // ALREADY ROTATED
        if (dir.Equals(curTargetDir)) return;
        curTargetDir = dir;

        // STOP ROTATE
        if (dir.IsZero) {
            tween.Pause();
            return;
        }

        float yDeg = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        // DO ROTATE
        tween.ChangeEndValue(
                new Vector3(0, yDeg, 0)
              , Mathf.Abs(Mathf.DeltaAngle(yDeg, transform.eulerAngles.y)) / RotationConfig.Instance.speed
              , true)
            .Restart();
    }
}
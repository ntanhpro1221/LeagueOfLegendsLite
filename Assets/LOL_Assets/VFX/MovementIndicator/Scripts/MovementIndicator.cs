using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovementIndicator : MonoBehaviour {
    [SerializeField] private List<Transform> arrows;
    [SerializeField] private Transform       ring;

    private List<Tweener> arrowTweeners = new();
    private Tweener       ringTweener;

    private void Awake() {
        var poolMan = MovementIndicatorPoolingManager.Instance;

        foreach (var arrow in arrows)
            arrowTweeners.Add(arrow
                .DOLocalRotate(new Vector3(poolMan.arrowRollEnd, 0, 0), poolMan.duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .SetAutoKill(false)
                .Pause());

        ringTweener = ring
            .DOScale(poolMan.ringScaleEnd, poolMan.duration)
            .SetEase(Ease.OutCubic)
            .SetAutoKill(false)
            .Pause();

        ringTweener.onComplete += () => MovementIndicatorPoolingManager.ImDone(this);
    }

    public void Restart() {
        foreach (var arrow in arrowTweeners) arrow.Restart();

        ringTweener.Restart();
    }
}
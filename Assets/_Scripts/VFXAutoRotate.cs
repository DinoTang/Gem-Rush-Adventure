using DG.Tweening;
using UnityEngine;

public class VFXAutoRotate : BaseBehaviour
{
    [SerializeField] private float rotateDuration = 2f;
    protected override void OnEnable()
    {
        transform.DOKill();

        float targetZ = 360f;

        transform
            .DORotate(
                new Vector3(0f, 0f, targetZ),
                rotateDuration,
                RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    protected override void OnDisable()
    {
        transform.DOKill();
    }
}
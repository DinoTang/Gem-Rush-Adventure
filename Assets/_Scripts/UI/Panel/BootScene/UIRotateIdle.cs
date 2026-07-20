using DG.Tweening;
using UnityEngine;

public class UIRotateIdle : BaseBehaviour
{
    [SerializeField] private float duration = 12f;

    protected override void Start()
    {
        transform
            .DORotate(
                new Vector3(0, 0, -360),
                duration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    protected override void OnDestroy()
    {
        transform.DOKill();
    }
}
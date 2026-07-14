using DG.Tweening;
using UnityEngine;

public class CompleteTickUI : BaseBehaviour
{
    [SerializeField] private float showDuration = 0.2f;

    public void Show()
    {
        gameObject.SetActive(true);

        transform.DOKill();
        transform.localScale = Vector3.zero;

        transform
            .DOScale(Vector3.one, showDuration)
            .SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        transform.DOKill();
    }
}
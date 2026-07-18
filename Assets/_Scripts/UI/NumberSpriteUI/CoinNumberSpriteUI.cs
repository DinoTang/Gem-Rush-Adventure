using DG.Tweening;
using UnityEngine;

public class CoinNumberSpriteUI : NumberSpriteUI
{
    [SerializeField] private float countDuration = 1.5f;

    private Tween countTween;

    protected override void LoadNumberSprites()
    {
        this.LoadCoinNumberSprites();
    }

    protected override void Start()
    {
        base.Start();

        this.SetNumber(0);
    }

    protected void LoadCoinNumberSprites()
    {
        if (this.numberSpriteSO != null)
            return;

        this.numberSpriteSO =
            Resources.Load<NumberSpriteSO>(
                "NumberSpriteSO/CoinNumberSprites"
            );

        Debug.Log(
            transform.name + ": LoadCoinNumberSprites",
            gameObject
        );
    }

    public void PlayCountAnimation(
    int targetScore,
    System.Action<int> onValueChanged = null,
    System.Action onCompleted = null)
    {
        countTween?.Kill();

        int displayedScore = 0;

        SetNumber(0);

        countTween = DOTween.To(
            () => displayedScore,
            value =>
            {
                displayedScore = value;

                SetNumber(value);

                onValueChanged?.Invoke(value);
            },
            targetScore,
            countDuration
        )
        .SetEase(Ease.OutCubic)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            SetNumber(targetScore);

            onCompleted?.Invoke();

            countTween = null;
        });
    }

    protected override void OnDisable()
    {
        this.countTween?.Kill();
        this.countTween = null;

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.countTween?.Kill();
        this.countTween = null;

        base.OnDestroy();
    }


}
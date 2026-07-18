using DG.Tweening;
using UnityEngine;

public class WinScoreNumberSpriteUI : NumberSpriteUI
{
    [SerializeField] private float countDuration = 1.5f;

    private Tween countTween;

    protected override void LoadNumberSprites()
    {
        this.LoadScoreNumberSprites();
    }

    protected override void Start()
    {
        base.Start();

        this.SetNumber(0);
    }

    protected void LoadScoreNumberSprites()
    {
        if (this.numberSpriteSO != null)
            return;

        this.numberSpriteSO =
            Resources.Load<NumberSpriteSO>(
                "NumberSpriteSO/ScoreNumberSprites"
            );

        Debug.Log(
            transform.name + ": Load Score Number Sprites",
            gameObject
        );
    }

    public void PlayCountAnimation(int targetScore)
    {
        this.countTween?.Kill();

        int displayedScore = 0;
        this.SetNumber(0);

        this.countTween = DOTween.To(
                () => displayedScore,
                value =>
                {
                    displayedScore = value;
                    this.SetNumber(displayedScore);
                },
                targetScore,
                this.countDuration
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                this.SetNumber(targetScore);
                this.countTween = null;
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
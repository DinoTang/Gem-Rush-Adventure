using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinStarUI : BaseBehaviour
{
    [Header("Components")]
    [SerializeField] private Image starImage;
    [SerializeField] private RectTransform starRect;

    [Header("Sprites")]
    [SerializeField] private Sprite star3Sprite;
    [SerializeField] private Sprite star4Sprite;
    [SerializeField] private Sprite star1Sprite;

    [Header("Animation")]
    [SerializeField] private float startScale = 2.5f;
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float settleDuration = 0.15f;

    private Sequence animationSequence;
    protected override void Start()
    {
        base.Start();
        this.ResetStar();
        this.PlayUnlockAnimation();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();

        if (this.starImage == null)
            this.starImage = GetComponent<Image>();

        if (this.starRect == null)
            this.starRect = GetComponent<RectTransform>();
    }

    public void PlayUnlockAnimation()
    {
        this.KillAnimation();

        gameObject.SetActive(true);

        this.starImage.sprite = this.star3Sprite;
        this.starRect.localScale =
            Vector3.one * this.startScale;

        this.starRect.localRotation =
            Quaternion.Euler(0f, 0f, -15f);

        this.animationSequence = DOTween.Sequence();

        // Thu ngôi sao từ lớn về gần kích thước thật.
        this.animationSequence.Append(
            this.starRect
                .DOScale(1.15f, this.shrinkDuration)
                .SetEase(Ease.OutCubic)
        );

        // Xoay nhẹ về vị trí thẳng.
        this.animationSequence.Join(
            this.starRect
                .DOLocalRotate(
                    Vector3.zero,
                    this.shrinkDuration
                )
                .SetEase(Ease.OutCubic)
        );

        // Đổi sprite trong lúc đang scale nhỏ.
        this.animationSequence.InsertCallback(
            this.shrinkDuration * 0.35f,
            () => this.starImage.sprite =
                this.star4Sprite
        );

        this.animationSequence.InsertCallback(
            this.shrinkDuration * 0.7f,
            () => this.starImage.sprite =
                this.star1Sprite
        );

        // Nảy nhẹ rồi khớp đúng kích thước khung.
        this.animationSequence.Append(
            this.starRect
                .DOScale(1f, this.settleDuration)
                .SetEase(Ease.OutBack)
        );

        this.animationSequence.SetUpdate(true);
    }

    public void ResetStar()
    {
        this.KillAnimation();

        this.starImage.sprite = this.star3Sprite;
        this.starRect.localScale = Vector3.zero;
        this.starRect.localRotation = Quaternion.identity;
    }

    private void KillAnimation()
    {
        this.animationSequence?.Kill();
        this.animationSequence = null;

        this.starRect?.DOKill();
    }

    protected override void OnDisable()
    {
        this.KillAnimation();
        base.OnDisable();
    }
}
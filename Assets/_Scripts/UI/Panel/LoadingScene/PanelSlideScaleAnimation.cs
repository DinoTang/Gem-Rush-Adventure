using DG.Tweening;
using UnityEngine;
public enum PanelSlideDirection
{
    Left,
    Right
}
public class PanelSlideScaleAnimation : BaseBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform panelRect;

    [Header("Slide")]
    [SerializeField]
    private PanelSlideDirection direction =
        PanelSlideDirection.Left;

    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float slideDistance = 900f;
    [SerializeField] private float slideDuration = 0.55f;
    [SerializeField]
    private Ease slideEase =
        Ease.OutCubic;

    [Header("Scale")]
    [SerializeField] private float scaleMultiplier = 1.12f;
    [SerializeField] private float scaleUpDuration = 0.16f;
    [SerializeField] private float scaleDownDuration = 0.14f;

    [SerializeField]
    private Ease scaleUpEase =
        Ease.OutQuad;

    [SerializeField]
    private Ease scaleDownEase =
        Ease.OutBack;

    [Header("Settings")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool useUnscaledTime = true;

    private Vector2 originalPosition;
    private Vector3 originalScale;

    private Sequence animationSequence;
    private bool isInitialized;

    protected override void LoadComponent()
    {
        base.LoadComponent();

        if (this.panelRect == null)
        {
            this.panelRect =
                GetComponent<RectTransform>();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        this.CacheOriginalState();
    }

    protected override void Start()
    {
        base.Start();

        if (!this.isInitialized)
        {
            this.CacheOriginalState();
        }

        if (this.playOnEnable)
        {
            this.PlayAnimation();
        }
    }

    private void CacheOriginalState()
    {
        if (this.panelRect == null)
            return;

        this.originalPosition =
            this.panelRect.anchoredPosition;

        this.originalScale =
            this.panelRect.localScale;

        this.isInitialized = true;
    }

    public void PlayAnimation()
    {
        if (this.panelRect == null)
            return;

        this.KillAnimation();

        float directionMultiplier =
            this.direction ==
            PanelSlideDirection.Left
                ? -1f
                : 1f;

        Vector2 startPosition =
            this.originalPosition +
            Vector2.right *
            this.slideDistance *
            directionMultiplier;

        this.panelRect.anchoredPosition =
            startPosition;

        this.panelRect.localScale =
            this.originalScale;

        Vector3 enlargedScale =
            this.originalScale *
            this.scaleMultiplier;

        this.animationSequence =
            DOTween.Sequence();

        this.animationSequence
            .SetUpdate(this.useUnscaledTime);

        this.animationSequence.AppendInterval(
            this.startDelay
        );

        // Bay vào vị trí gốc.
        this.animationSequence.Append(
            this.panelRect
                .DOAnchorPos(
                    this.originalPosition,
                    this.slideDuration
                )
                .SetEase(this.slideEase)
        );

        // Scale lớn nhẹ.
        this.animationSequence.Append(
            this.panelRect
                .DOScale(
                    enlargedScale,
                    this.scaleUpDuration
                )
                .SetEase(this.scaleUpEase)
        );

        // Scale về kích thước ban đầu.
        this.animationSequence.Append(
            this.panelRect
                .DOScale(
                    this.originalScale,
                    this.scaleDownDuration
                )
                .SetEase(this.scaleDownEase)
        );

        this.animationSequence.OnComplete(() =>
        {
            this.panelRect.anchoredPosition =
                this.originalPosition;

            this.panelRect.localScale =
                this.originalScale;

            this.animationSequence = null;
        });
    }

    public void ResetPanel()
    {
        this.KillAnimation();

        if (this.panelRect == null)
            return;

        this.panelRect.anchoredPosition =
            this.originalPosition;

        this.panelRect.localScale =
            this.originalScale;
    }

    private void KillAnimation()
    {
        this.animationSequence?.Kill();
        this.animationSequence = null;

        this.panelRect?.DOKill();
    }

    protected override void OnDisable()
    {
        this.KillAnimation();

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.KillAnimation();

        base.OnDestroy();
    }
}
using DG.Tweening;
using UnityEngine;

public class PanelScaleAnimation : BaseBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform panelRect;

    [Header("Animation")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float overshootScale = 1.15f;
    [SerializeField] private float scaleUpDuration = 0.35f;
    [SerializeField] private float settleDuration = 0.18f;

    [SerializeField]
    private Ease scaleUpEase =
        Ease.OutBack;

    [SerializeField]
    private Ease settleEase =
        Ease.OutCubic;

    [Header("Settings")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool useUnscaledTime = true;

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

        this.originalScale =
            this.panelRect.localScale;

        this.isInitialized = true;
    }

    public void PlayAnimation()
    {
        if (this.panelRect == null)
            return;

        this.KillAnimation();

        this.panelRect.localScale =
            Vector3.zero;

        Vector3 overshootTarget =
            this.originalScale *
            this.overshootScale;

        this.animationSequence =
            DOTween.Sequence();

        this.animationSequence
            .SetUpdate(this.useUnscaledTime);

        this.animationSequence.AppendInterval(
            this.startDelay
        );

        // Scale từ 0 lên lớn hơn kích thước gốc.
        this.animationSequence.Append(
            this.panelRect
                .DOScale(
                    overshootTarget,
                    this.scaleUpDuration
                )
                .SetEase(this.scaleUpEase)
        );

        // Co về đúng kích thước gốc.
        this.animationSequence.Append(
            this.panelRect
                .DOScale(
                    this.originalScale,
                    this.settleDuration
                )
                .SetEase(this.settleEase)
        );

        this.animationSequence.OnComplete(() =>
        {
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

        this.panelRect.localScale =
            Vector3.zero;
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
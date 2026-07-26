using System;
using DG.Tweening;
using UnityEngine;

public abstract class EndGameMessageUI : BaseUI
{
    [Header("Components")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform titleRect;

    [Header("Animation")]
    [SerializeField] protected float fadeDuration = 0.2f;
    [SerializeField] protected float pulseScale = 1.08f;
    [SerializeField] protected float pulseDuration = 0.6f;

    protected Tween pulseTween;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadCanvasGroup();
        this.LoadTitleRect();
    }

    protected void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;

        this.canvasGroup = GetComponent<CanvasGroup>();
        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }

    protected abstract void LoadTitleRect();

    protected override void Start()
    {
        base.Start();
        this.InitializeHidden();
    }

    public override void Show()
    {
        base.Show();

        this.KillTweens();

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.titleRect.localScale = Vector3.one;

        this.canvasGroup.DOFade(1f, this.fadeDuration).SetUpdate(true);

        this.pulseTween = this.titleRect
            .DOScale(this.pulseScale, this.pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public override void Hide()
    {
        this.Hide(null);
    }

    public void Hide(Action onCompleted)
    {
        this.KillTweens();

        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.canvasGroup
            .DOFade(0f, this.fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                base.Hide();
                onCompleted?.Invoke();
            });
    }

    protected void InitializeHidden()
    {
        this.KillTweens();

        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0f;
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
        }

        if (this.titleRect != null)
            this.titleRect.localScale = Vector3.one;
    }

    protected void SetRaycastState(bool value)
    {
        if (this.canvasGroup == null) return;

        this.canvasGroup.interactable = value;
        this.canvasGroup.blocksRaycasts = value;
    }

    protected virtual void KillTweens()
    {
        this.canvasGroup?.DOKill();
        this.titleRect?.DOKill();

        this.pulseTween?.Kill();
        this.pulseTween = null;
    }

    protected override void OnDisable()
    {
        this.KillTweens();
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.KillTweens();
        base.OnDestroy();
    }
}
using DG.Tweening;
using UnityEngine;

public abstract class AnimatedPanelUI : BaseUI
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rectTransform;

    [SerializeField] protected float moveDistance = 40f;
    [SerializeField] protected float showDuration = 0.2f;
    [SerializeField] protected float hideDuration = 0.15f;

    protected Vector2 originalPosition;

    protected virtual Vector2 MoveOffset => new Vector2(moveDistance, 0f);
    protected override void Awake()
    {
        base.Awake();
        this.originalPosition = this.rectTransform.anchoredPosition;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadRectTransform();
        this.LoadCanvasGroup();
    }
    protected void LoadRectTransform()
    {
        if (this.rectTransform != null) return;
        this.rectTransform = GetComponent<RectTransform>();
        Debug.Log(transform.name + ": LoadRectTransform", gameObject);
    }
    protected void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;
        this.canvasGroup = GetComponent<CanvasGroup>();
        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }
    public virtual void ShowAnimated()
    {
        base.Show();

        this.rectTransform.DOKill();
        this.canvasGroup.DOKill();

        this.rectTransform.anchoredPosition = this.originalPosition + this.MoveOffset;
        this.canvasGroup.alpha = 0f;

        this.rectTransform
            .DOAnchorPos(this.originalPosition, this.showDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        this.canvasGroup
            .DOFade(1f, this.showDuration)
            .SetUpdate(true);
    }

    public virtual void HideAnimated()
    {
        this.rectTransform.DOKill();
        this.canvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence()
            .SetUpdate(true);

        sequence.Join(
            this.rectTransform
                .DOAnchorPos(this.originalPosition + this.MoveOffset, this.hideDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.Join(this.canvasGroup.DOFade(0f, this.hideDuration));

        sequence.OnComplete(() =>
        {
            this.rectTransform.anchoredPosition = this.originalPosition;
            base.Hide();
        });
    }

    public override void Show()
    {
        base.Show();

        this.rectTransform.DOKill();
        this.canvasGroup.DOKill();

        this.canvasGroup.alpha = 1f;
        this.rectTransform.anchoredPosition = this.originalPosition;
    }

    public override void Hide()
    {
        this.rectTransform.DOKill();
        this.canvasGroup.DOKill();

        this.canvasGroup.alpha = 0f;
        this.rectTransform.anchoredPosition = this.originalPosition;
        base.Hide();
    }
}

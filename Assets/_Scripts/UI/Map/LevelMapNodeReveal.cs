using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapNodeReveal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected ScrollRect scrollRect;
    [SerializeField] protected RectTransform targetRect;
    [SerializeField] protected Canvas canvas;

    [Header("Detection")]
    [SerializeField] protected float viewportPadding = 150f;

    [Header("Animation")]
    [SerializeField] protected float overshootScale = 1.15f;
    [SerializeField] protected float scaleUpDuration = 0.25f;
    [SerializeField] protected float settleDuration = 0.12f;

    [Header("Behaviour")]
    [SerializeField] protected bool replayWhenLeavingViewport;

    protected bool hasRevealed;
    protected bool isNearViewport;
    protected Tween revealTween;

    protected virtual void Awake()
    {
        this.LoadComponents();

        this.targetRect.localScale = Vector3.zero;
    }

    protected virtual void Update()
    {
        bool nearViewport = this.IsNearViewport();

        if (nearViewport && !this.isNearViewport)
        {
            this.OnEnterNearViewport();
        }
        else if (!nearViewport && this.isNearViewport)
        {
            this.OnExitNearViewport();
        }

        this.isNearViewport = nearViewport;
    }

    protected virtual void OnDestroy()
    {
        this.revealTween?.Kill();
    }

    protected void LoadComponents()
    {
        if (this.targetRect == null)
        {
            this.targetRect = GetComponent<RectTransform>();
        }

        if (this.scrollRect == null)
        {
            this.scrollRect = GameObject
                .Find("Scroll View")
                .GetComponent<ScrollRect>();
        }

        if (this.canvas == null)
        {
            this.canvas = GameObject
                .Find("UI")
                .GetComponent<Canvas>();
        }
    }

    protected bool IsNearViewport()
    {
        if (this.scrollRect == null ||
            this.scrollRect.viewport == null ||
            this.targetRect == null)
        {
            return false;
        }

        Camera uiCamera = this.GetUICamera();

        Vector2 nodeScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                this.targetRect.position
            );

        Vector3[] viewportCorners = new Vector3[4];
        this.scrollRect.viewport.GetWorldCorners(viewportCorners);

        Vector2 bottomLeft =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                viewportCorners[0]
            );

        Vector2 topRight =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                viewportCorners[2]
            );

        Rect expandedViewport = new Rect(
            bottomLeft.x - this.viewportPadding,
            bottomLeft.y - this.viewportPadding,
            topRight.x - bottomLeft.x + this.viewportPadding * 2f,
            topRight.y - bottomLeft.y + this.viewportPadding * 2f
        );

        return expandedViewport.Contains(nodeScreenPosition);
    }

    protected Camera GetUICamera()
    {
        if (this.canvas == null) return null;

        if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return this.canvas.worldCamera;
    }

    protected void OnEnterNearViewport()
    {
        if (this.hasRevealed && !this.replayWhenLeavingViewport)
        {
            return;
        }

        this.PlayRevealAnimation();
        this.hasRevealed = true;
    }

    protected void OnExitNearViewport()
    {
        if (!this.replayWhenLeavingViewport)
        {
            return;
        }

        this.revealTween?.Kill();

        this.targetRect.localScale = Vector3.zero;
        this.hasRevealed = false;
    }

    protected void PlayRevealAnimation()
    {
        this.revealTween?.Kill();

        this.targetRect.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            this.targetRect
                .DOScale(this.overshootScale, this.scaleUpDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Append(
            this.targetRect
                .DOScale(Vector3.one, this.settleDuration)
                .SetEase(Ease.OutQuad)
        );

        this.revealTween = sequence;
    }
}
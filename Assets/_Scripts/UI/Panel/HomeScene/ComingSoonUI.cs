using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ComingSoonUI : BaseUI
{
    private static ComingSoonUI instance;
    public static ComingSoonUI Instance => instance;

    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image cloudImage;
    [SerializeField] private Image comingSoonImage;
    [SerializeField] private RectTransform cloudRect;
    [SerializeField] private RectTransform comingSoonRect;

    [Header("Show Animation")]
    [SerializeField] private float startOffsetY = 50f;
    [SerializeField] private float fadeInDuration = 0.12f;
    [SerializeField] private float cloudShowDuration = 0.3f;
    [SerializeField] private float textShowDuration = 0.22f;
    [SerializeField] private float cloudOvershootScale = 1.1f;
    [SerializeField] private float cloudSettleDuration = 0.12f;

    [Header("Idle")]
    [SerializeField] private float visibleDuration = 1f;
    [SerializeField] private float textPunchScale = 0.1f;
    [SerializeField] private float textPunchDuration = 0.25f;

    [Header("Hide Animation")]
    [SerializeField] private float hideOffsetY = 30f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    [Header("Settings")]
    [SerializeField] private bool useUnscaledTime = true;

    private Sequence animationSequence;

    private Vector2 cloudOriginalPosition;
    private Vector2 textOriginalPosition;

    private Vector3 cloudOriginalScale;
    private Vector3 textOriginalScale;

    protected override void Awake()
    {
        base.Awake();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();

        this.LoadCanvasGroup();
        this.LoadCloudImage();
        this.LoadComingSoonImage();
        this.LoadCloudRect();
        this.LoadComingSoonRect();
    }

    protected override void Start()
    {
        base.Start();

        this.CacheOriginalState();
        this.HideImmediately();
    }

    protected void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;

        this.canvasGroup = GetComponent<CanvasGroup>();
        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }

    protected void LoadCloudImage()
    {
        if (this.cloudImage != null) return;

        this.cloudImage = GetComponentInChildren<CloudImageUI>(true)?.GetComponent<Image>();

        if (this.cloudImage == null)
            Debug.LogWarning(transform.name + ": CloudImage not found", gameObject);
    }

    protected void LoadComingSoonImage()
    {
        if (this.comingSoonImage != null) return;

        this.comingSoonImage = GetComponentInChildren<ComingSoonTitleUI>(true)?.GetComponent<Image>();

        if (this.comingSoonImage == null)
            Debug.LogWarning(transform.name + ": ComingSoonImage not found", gameObject);
    }

    protected void LoadCloudRect()
    {
        if (this.cloudRect != null) return;
        if (this.cloudImage == null) return;

        this.cloudRect = this.cloudImage.GetComponent<RectTransform>();
        Debug.Log(transform.name + ": LoadCloudRect", gameObject);
    }

    protected void LoadComingSoonRect()
    {
        if (this.comingSoonRect != null) return;
        if (this.comingSoonImage == null) return;

        this.comingSoonRect = this.comingSoonImage.GetComponent<RectTransform>();
        Debug.Log(transform.name + ": LoadComingSoonRect", gameObject);
    }

    private void CacheOriginalState()
    {
        if (this.cloudRect != null)
        {
            this.cloudOriginalPosition = this.cloudRect.anchoredPosition;
            this.cloudOriginalScale = this.cloudRect.localScale;
        }

        if (this.comingSoonRect != null)
        {
            this.textOriginalPosition = this.comingSoonRect.anchoredPosition;
            this.textOriginalScale = this.comingSoonRect.localScale;
        }
    }

    public override void Show()
    {
        if (this.canvasGroup == null || this.cloudRect == null || this.comingSoonRect == null)
        {
            Debug.LogError(transform.name + ": ComingSoonUI is missing references", gameObject);
            return;
        }

        base.Show();
        this.KillAnimation();

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.cloudRect.anchoredPosition = this.cloudOriginalPosition - Vector2.up * this.startOffsetY;
        this.cloudRect.localScale = Vector3.zero;

        this.comingSoonRect.anchoredPosition = this.textOriginalPosition;
        this.comingSoonRect.localScale = Vector3.zero;

        Vector3 cloudOvershoot = this.cloudOriginalScale * this.cloudOvershootScale;

        this.animationSequence = DOTween.Sequence();
        this.animationSequence.SetUpdate(this.useUnscaledTime);

        this.animationSequence.Append(
            this.canvasGroup.DOFade(1f, this.fadeInDuration).SetEase(Ease.OutQuad)
        );

        this.animationSequence.Join(
            this.cloudRect.DOAnchorPos(this.cloudOriginalPosition, this.cloudShowDuration)
                .SetEase(Ease.OutBack)
        );

        this.animationSequence.Join(
            this.cloudRect.DOScale(cloudOvershoot, this.cloudShowDuration)
                .SetEase(Ease.OutBack)
        );

        this.animationSequence.Append(
            this.cloudRect.DOScale(this.cloudOriginalScale, this.cloudSettleDuration)
                .SetEase(Ease.OutCubic)
        );

        this.animationSequence.Join(
            this.comingSoonRect.DOScale(this.textOriginalScale, this.textShowDuration)
                .SetEase(Ease.OutBack)
        );

        this.animationSequence.Append(
            this.comingSoonRect.DOPunchScale(
                Vector3.one * this.textPunchScale,
                this.textPunchDuration,
                5,
                0.5f
            )
        );

        this.animationSequence.AppendInterval(this.visibleDuration);

        this.animationSequence.Append(
            this.canvasGroup.DOFade(0f, this.fadeOutDuration).SetEase(Ease.InQuad)
        );

        this.animationSequence.Join(
            this.cloudRect.DOAnchorPos(
                this.cloudOriginalPosition + Vector2.up * this.hideOffsetY,
                this.fadeOutDuration
            ).SetEase(Ease.InCubic)
        );

        this.animationSequence.Join(
            this.comingSoonRect.DOAnchorPos(
                this.textOriginalPosition + Vector2.up * this.hideOffsetY,
                this.fadeOutDuration
            ).SetEase(Ease.InCubic)
        );

        this.animationSequence.OnComplete(() =>
        {
            this.RestoreOriginalState();
            this.animationSequence = null;
            base.Hide();
        });
    }

    public void HideImmediately()
    {
        this.KillAnimation();

        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0f;
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
        }

        this.RestoreOriginalState();
        base.Hide();
    }

    private void RestoreOriginalState()
    {
        if (this.cloudRect != null)
        {
            this.cloudRect.anchoredPosition = this.cloudOriginalPosition;
            this.cloudRect.localScale = this.cloudOriginalScale;
        }

        if (this.comingSoonRect != null)
        {
            this.comingSoonRect.anchoredPosition = this.textOriginalPosition;
            this.comingSoonRect.localScale = this.textOriginalScale;
        }
    }

    private void KillAnimation()
    {
        this.animationSequence?.Kill();
        this.animationSequence = null;

        this.canvasGroup?.DOKill();
        this.cloudRect?.DOKill();
        this.comingSoonRect?.DOKill();
    }

    protected override void OnDisable()
    {
        this.KillAnimation();
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        if (instance == this)
            instance = null;

        this.KillAnimation();
        base.OnDestroy();
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContinueBtnUI : BaseBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Components")]
    [SerializeField] private Button continueButton;
    [SerializeField] private RectTransform buttonRect;

    [Header("Idle Animation")]
    [SerializeField] private float idleScale = 1.05f;
    [SerializeField] private float idleMoveY = 5f;
    [SerializeField] private float idleDuration = 0.7f;

    [Header("Press Animation")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float releaseDuration = 0.15f;

    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool useUnscaledTime = true;

    private Sequence idleSequence;
    private Tween pressTween;

    private Vector2 originalAnchoredPosition;
    private Vector3 originalScale;

    private bool isInitialized;
    private bool isIdlePlaying;

    protected override void LoadComponent()
    {
        base.LoadComponent();

        if (this.continueButton == null)
            this.continueButton = GetComponent<Button>();

        if (this.buttonRect == null)
            this.buttonRect = GetComponent<RectTransform>();
    }

    protected override void Awake()
    {
        base.Awake();

        this.CacheOriginalTransform();
    }

    protected override void Start()
    {
        base.Start();

        if (!this.isInitialized)
            this.CacheOriginalTransform();

        if (this.playOnStart)
            this.PlayIdleAnimation();
    }

    private void CacheOriginalTransform()
    {
        if (this.buttonRect == null)
            return;

        this.originalAnchoredPosition =
            this.buttonRect.anchoredPosition;

        this.originalScale =
            this.buttonRect.localScale;

        this.isInitialized = true;
    }

    public void PlayIdleAnimation()
    {
        if (this.buttonRect == null)
            return;

        this.KillAnimations();

        this.isIdlePlaying = true;

        this.buttonRect.anchoredPosition =
            this.originalAnchoredPosition;

        this.buttonRect.localScale =
            this.originalScale;

        Vector3 targetScale =
            this.originalScale * this.idleScale;

        float targetY =
            this.originalAnchoredPosition.y +
            this.idleMoveY;

        this.idleSequence = DOTween.Sequence();

        this.idleSequence.Append(
            this.buttonRect
                .DOScale(
                    targetScale,
                    this.idleDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.idleSequence.Join(
            this.buttonRect
                .DOAnchorPosY(
                    targetY,
                    this.idleDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.idleSequence.Append(
            this.buttonRect
                .DOScale(
                    this.originalScale,
                    this.idleDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.idleSequence.Join(
            this.buttonRect
                .DOAnchorPosY(
                    this.originalAnchoredPosition.y,
                    this.idleDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.idleSequence
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(this.useUnscaledTime);
    }

    public void StopIdleAnimation(bool resetTransform = true)
    {
        this.isIdlePlaying = false;

        this.idleSequence?.Kill();
        this.idleSequence = null;

        if (!resetTransform || this.buttonRect == null)
            return;

        this.buttonRect.anchoredPosition =
            this.originalAnchoredPosition;

        this.buttonRect.localScale =
            this.originalScale;
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (this.continueButton != null &&
            !this.continueButton.interactable)
        {
            return;
        }

        this.idleSequence?.Pause();

        this.pressTween?.Kill();

        this.pressTween = this.buttonRect
            .DOScale(
                this.originalScale *
                this.pressedScale,
                this.pressDuration
            )
            .SetEase(Ease.OutQuad)
            .SetUpdate(this.useUnscaledTime);
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        if (this.continueButton != null &&
            !this.continueButton.interactable)
        {
            return;
        }

        this.pressTween?.Kill();

        this.pressTween = this.buttonRect
            .DOScale(
                this.originalScale,
                this.releaseDuration
            )
            .SetEase(Ease.OutBack)
            .SetUpdate(this.useUnscaledTime)
            .OnComplete(() =>
            {
                if (this.isIdlePlaying)
                    this.idleSequence?.Play();
            });
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (this.continueButton != null &&
            !this.continueButton.interactable)
        {
            return;
        }

        Debug.Log("Continue button clicked", this);

        this.StopIdleAnimation();

        // Thêm logic chuyển màn ở đây.
        // Ví dụ:
        // SceneManager.LoadScene("LevelSelect");
    }

    public void SetInteractable(bool value)
    {
        if (this.continueButton == null)
            return;

        this.continueButton.interactable = value;

        if (value)
        {
            this.PlayIdleAnimation();
        }
        else
        {
            this.StopIdleAnimation();
        }
    }

    private void KillAnimations()
    {
        this.idleSequence?.Kill();
        this.idleSequence = null;

        this.pressTween?.Kill();
        this.pressTween = null;

        this.buttonRect?.DOKill();
    }

    protected override void OnDisable()
    {
        this.KillAnimations();

        if (this.buttonRect != null &&
            this.isInitialized)
        {
            this.buttonRect.anchoredPosition =
                this.originalAnchoredPosition;

            this.buttonRect.localScale =
                this.originalScale;
        }

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        this.KillAnimations();

        base.OnDestroy();
    }
}
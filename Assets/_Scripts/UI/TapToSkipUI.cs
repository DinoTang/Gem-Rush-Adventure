using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapToSkipUI : BaseUI, IPointerClickHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform tapTitle;
    [SerializeField] protected WinPopupUI winPopupUI;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float pulseScale = 1.08f;
    [SerializeField] private float pulseDuration = 0.6f;

    private Tween pulseTween;
    [SerializeField] private bool canContinue;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadCanvasGroup();
        this.LoadTapTitle();
        this.LoadWinPopupUI();
    }

    protected void LoadCanvasGroup()
    {
        if (this.canvasGroup != null) return;

        this.canvasGroup = GetComponent<CanvasGroup>();

        Debug.Log(transform.name + ": LoadCanvasGroup", gameObject);
    }

    protected void LoadTapTitle()
    {
        if (this.tapTitle != null) return;

        this.tapTitle = GetComponentInChildren<TapToSkipTitleUI>().GetComponent<RectTransform>();

        Debug.Log(transform.name + ": LoadTapTitle", gameObject);
    }

    protected void LoadWinPopupUI()
    {
        if (this.winPopupUI != null) return;

        this.winPopupUI = FindAnyObjectByType<WinPopupUI>();
        this.winPopupUI.Hide();
        Debug.Log(transform.name + ": LoadWinPopupUI", gameObject);
    }

    protected override void Start()
    {
        base.Start();
        this.InitializeHidden();
    }

    public override void Show()
    {
        base.Show();

        this.KillTweens();

        this.canContinue = false;

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = true;
        this.canvasGroup.blocksRaycasts = true;

        this.tapTitle.localScale = Vector3.one;

        this.canvasGroup
            .DOFade(1f, this.fadeDuration)
            .SetUpdate(true);

        this.pulseTween = this.tapTitle
            .DOScale(this.pulseScale, this.pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public void EnableContinue()
    {
        this.canContinue = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!this.canContinue)
            return;


        this.canContinue = false;
        this.Hide();

        GamePlayUI.Instance.Hide();
        this.winPopupUI.Show();
        Debug.LogWarning("Tap To Continue clicked");
    }

    public override void Hide()
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
            });
    }
    private void InitializeHidden()
    {
        this.KillTweens();

        this.canContinue = false;

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.tapTitle.localScale = Vector3.one;
    }
    // private void HideImmediately()
    // {
    //     this.KillTweens();

    //     this.canvasGroup.alpha = 0f;
    //     this.canvasGroup.interactable = false;
    //     this.canvasGroup.blocksRaycasts = false;

    //     base.Hide();
    // }

    private void KillTweens()
    {
        this.canvasGroup?.DOKill();
        this.tapTitle?.DOKill();

        this.pulseTween?.Kill();
        this.pulseTween = null;
    }

    protected override void OnDisable()
    {
        this.KillTweens();
        base.OnDisable();
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapToStartUI : BaseUI, IPointerClickHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform tapTitle;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float pulseScale = 1.08f;
    [SerializeField] private float pulseDuration = 0.6f;

    private Tween pulseTween;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadCanvasGroup();
        this.LoadTapTitle();
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

        this.tapTitle = GetComponentInChildren<TapToStartTitleUI>().GetComponent<RectTransform>();

        Debug.Log(transform.name + ": LoadTapTitle", gameObject);
    }


    protected override void Start()
    {
        base.Start();
        // this.Show();
    }

    public override void Show()
    {
        base.Show();

        this.KillTweens();


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


    public void OnPointerClick(PointerEventData eventData)
    {
        LoadingSceneUI.Instance.ActivateLoadedScene();
        Debug.LogWarning("Tap To Start clicked");
    }


    private void InitializeHidden()
    {
        this.KillTweens();


        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        this.tapTitle.localScale = Vector3.one;
    }
    public void HideImmediately()
    {
        this.KillTweens();

        this.canvasGroup.alpha = 0f;
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;

        base.Hide();
    }

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
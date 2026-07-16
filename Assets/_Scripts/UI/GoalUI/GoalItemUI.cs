using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GoalItemUI : BaseBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private NumberSpriteUI amountUI;
    [SerializeField] private CompleteTickUI completeTickUI;

    [Header("Databases")]
    [SerializeField] private GemSpriteSO gemSpriteDatabase;

    [Header("Complete Animation")]
    [SerializeField] private float scaleUp = 1.15f;
    [SerializeField] private float scaleDuration = 0.15f;
    [SerializeField] private float rotateAngle = 10f;
    [SerializeField] private float rotateDuration = 0.08f;

    private LevelGoalProgress progress;
    private Sequence completeSequence;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private bool hasCompletedEffectPlayed;

    public LevelGoalProgress Progress => progress;

    protected override void Awake()
    {
        base.Awake();

        originalScale = this.icon.rectTransform.localScale;
        originalRotation = this.icon.rectTransform.localEulerAngles;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();

        LoadIcon();
        LoadAmountUI();
        LoadCompleteTickUI();
        LoadGemSpriteSO();
    }

    protected void LoadIcon()
    {
        if (this.icon != null)
            return;

        this.icon = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": LoadIcon", gameObject);
    }

    protected void LoadAmountUI()
    {
        if (this.amountUI != null)
            return;

        this.amountUI = GetComponentInChildren<NumberSpriteUI>();
        Debug.Log(transform.name + ": LoadAmountUI", gameObject);
    }

    protected void LoadCompleteTickUI()
    {
        if (this.completeTickUI != null)
            return;

        this.completeTickUI = GetComponentInChildren<CompleteTickUI>(true);
        Debug.Log(transform.name + ": LoadCompleteTickUI", gameObject);
    }

    protected void LoadGemSpriteSO()
    {
        if (this.gemSpriteDatabase != null)
            return;

        this.gemSpriteDatabase = Resources.Load<GemSpriteSO>("GemSpriteSO");
        Debug.Log(transform.name + ": LoadGemSpriteSO", gameObject);
    }

    public void Init(LevelGoalProgress goalProgress)
    {
        progress = goalProgress;

        this.ResetVisual();

        this.icon.sprite = this.gemSpriteDatabase.GetSprite(
            this.progress.Data.gemType,
            this.progress.Data.gemSpecialType
        );

        Refresh();
    }

    public void Refresh()
    {
        if (this.progress == null)
            return;

        if (this.progress.IsCompleted)
        {
            this.PlayCompleteEffect();
            return;
        }

        this.amountUI.SetNumber(this.progress.CurrentAmount);
    }

    private void PlayCompleteEffect()
    {
        if (this.hasCompletedEffectPlayed)
            return;

        this.hasCompletedEffectPlayed = true;

        this.completeSequence?.Kill();

        RectTransform iconRect = this.icon.rectTransform;

        this.completeSequence = DOTween.Sequence();

        this.completeSequence.Append(
           iconRect
               .DOScale(originalScale * scaleUp, scaleDuration)
               .SetEase(Ease.OutQuad)
       );

        this.completeSequence.Append(
            iconRect
                .DOLocalRotate(
                    originalRotation + new Vector3(0f, 0f, -rotateAngle),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.completeSequence.Append(
            iconRect
                .DOLocalRotate(
                    originalRotation + new Vector3(0f, 0f, rotateAngle),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        this.completeSequence.Append(
            iconRect
                .DOLocalRotate(originalRotation, rotateDuration)
                .SetEase(Ease.InOutSine)
        );

        this.completeSequence.Append(
            iconRect
                .DOScale(originalScale, scaleDuration)
                .SetEase(Ease.InQuad)
        );

        this.completeSequence.OnComplete(() =>
        {
            this.amountUI.gameObject.SetActive(false);
            this.completeTickUI.Show();
        });
    }

    private void ResetVisual()
    {
        this.completeSequence?.Kill();

        this.hasCompletedEffectPlayed = false;

        this.icon.rectTransform.localScale = this.originalScale;
        this.icon.rectTransform.localEulerAngles = this.originalRotation;

        this.amountUI.gameObject.SetActive(true);
        this.completeTickUI.Hide();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        this.completeSequence?.Kill();
        this.icon.rectTransform.DOKill();
    }
}
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

        originalScale = icon.rectTransform.localScale;
        originalRotation = icon.rectTransform.localEulerAngles;
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
        if (icon != null)
            return;

        icon = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": LoadIcon", gameObject);
    }

    protected void LoadAmountUI()
    {
        if (amountUI != null)
            return;

        amountUI = GetComponentInChildren<NumberSpriteUI>();
        Debug.Log(transform.name + ": LoadAmountUI", gameObject);
    }

    protected void LoadCompleteTickUI()
    {
        if (completeTickUI != null)
            return;

        completeTickUI = GetComponentInChildren<CompleteTickUI>(true);
        Debug.Log(transform.name + ": LoadCompleteTickUI", gameObject);
    }

    protected void LoadGemSpriteSO()
    {
        if (gemSpriteDatabase != null)
            return;

        gemSpriteDatabase = Resources.Load<GemSpriteSO>("GemSpriteSO");
        Debug.Log(transform.name + ": LoadGemSpriteSO", gameObject);
    }

    public void Init(LevelGoalProgress goalProgress)
    {
        progress = goalProgress;

        ResetVisual();

        icon.sprite = gemSpriteDatabase.GetSprite(
            progress.Data.gemType,
            progress.Data.gemSpecialType
        );

        Refresh();
    }

    public void Refresh()
    {
        if (progress == null)
            return;

        if (progress.IsCompleted)
        {
            PlayCompleteEffect();
            return;
        }

        amountUI.SetNumber(progress.CurrentAmount);
    }

    private void PlayCompleteEffect()
    {
        if (hasCompletedEffectPlayed)
            return;

        hasCompletedEffectPlayed = true;

        completeSequence?.Kill();

        RectTransform iconRect = icon.rectTransform;

        completeSequence = DOTween.Sequence();

        completeSequence.Append(
            iconRect
                .DOScale(originalScale * scaleUp, scaleDuration)
                .SetEase(Ease.OutQuad)
        );

        completeSequence.Append(
            iconRect
                .DOLocalRotate(
                    originalRotation + new Vector3(0f, 0f, -rotateAngle),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        completeSequence.Append(
            iconRect
                .DOLocalRotate(
                    originalRotation + new Vector3(0f, 0f, rotateAngle),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        completeSequence.Append(
            iconRect
                .DOLocalRotate(originalRotation, rotateDuration)
                .SetEase(Ease.InOutSine)
        );

        completeSequence.Append(
            iconRect
                .DOScale(originalScale, scaleDuration)
                .SetEase(Ease.InQuad)
        );

        completeSequence.OnComplete(() =>
        {
            amountUI.gameObject.SetActive(false);
            completeTickUI.Show();
        });
    }

    private void ResetVisual()
    {
        completeSequence?.Kill();

        hasCompletedEffectPlayed = false;

        icon.rectTransform.localScale = originalScale;
        icon.rectTransform.localEulerAngles = originalRotation;

        amountUI.gameObject.SetActive(true);
        completeTickUI.Hide();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        completeSequence?.Kill();
        icon.rectTransform.DOKill();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class GoalItemUI : BaseBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NumberSpriteUI amountUI;
    [SerializeField] private GemSpriteSO gemSpriteDatabase;

    private LevelGoalProgress progress;

    public LevelGoalProgress Progress => progress;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadIcon();
        this.LoadAmountUI();
        this.LoadGemSpriteSO();
    }

    protected void LoadIcon()
    {
        if (this.icon != null) return;

        this.icon = GetComponentInChildren<Image>();
        Debug.Log(transform.name + ": LoadIcon", gameObject);
    }

    protected void LoadAmountUI()
    {
        if (this.amountUI != null) return;

        this.amountUI = GetComponentInChildren<NumberSpriteUI>();
        Debug.Log(transform.name + ": LoadAmountUI", gameObject);
    }

    protected void LoadGemSpriteSO()
    {
        if (this.gemSpriteDatabase != null) return;

        this.gemSpriteDatabase = Resources.Load<GemSpriteSO>("GemSpriteSO");
        Debug.Log(transform.name + ": LoadGemSpriteSO", gameObject);
    }

    public void Init(LevelGoalProgress goalProgress)
    {
        progress = goalProgress;

        icon.sprite = gemSpriteDatabase.GetSprite(
            progress.Data.gemType, progress.Data.gemSpecialType
        );

        Refresh();
    }

    public void Refresh()
    {
        if (progress == null)
            return;

        // Nếu CurrentAmount đang lưu số còn lại
        amountUI.SetNumber(progress.CurrentAmount);
    }
}
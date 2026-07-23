using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapUI : BaseUI
{
    [SerializeField] protected LevelSO levelSO;
    [SerializeField] protected ButtonSpriteSO lockedLevelMapBtnSpriteSO;
    [SerializeField] protected ButtonSpriteSO noStarLevelMapBtnSpriteSO;
    [SerializeField] protected ButtonSpriteSO star1LevelMapBtnSpriteSO;
    [SerializeField] protected ButtonSpriteSO star2LevelMapBtnSpriteSO;
    [SerializeField] protected ButtonSpriteSO star3LevelMapBtnSpriteSO;
    [SerializeField] protected LevelNumberSpriteUI levelNumberSpriteUI;
    public LevelSO LevelSO => levelSO;

    public ButtonSpriteSO LockedLevelMapBtnSpriteSO => lockedLevelMapBtnSpriteSO;
    public ButtonSpriteSO NoStarLevelMapBtnSpriteSO => noStarLevelMapBtnSpriteSO;
    public ButtonSpriteSO Star1LevelMapBtnSpriteSO => star1LevelMapBtnSpriteSO;
    public ButtonSpriteSO Star2LevelMapBtnSpriteSO => star2LevelMapBtnSpriteSO;
    public ButtonSpriteSO Star3LevelMapBtnSpriteSO => star3LevelMapBtnSpriteSO;
    public LevelNumberSpriteUI LevelNumberSpriteUI => levelNumberSpriteUI;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadLevelSO();
        this.LoadLockedLevelMapBtnSpriteSO();
        this.LoadNoStarLevelMapBtnSpriteSO();
        this.LoadStar1LevelMapBtnSpriteSO();
        this.LoadStar2LevelMapBtnSpriteSO();
        this.LoadStar3LevelMapBtnSpriteSO();
        this.LoadLevelNumberSpriteUI();
    }
    protected void LoadLevelSO()
    {
        if (this.levelSO != null) return;
        this.levelSO = Resources.Load<LevelSO>("LevelData/" + transform.name);
        Debug.Log(transform.name + ": LoadLevelSO");
    }
    protected void LoadLockedLevelMapBtnSpriteSO()
    {
        if (this.lockedLevelMapBtnSpriteSO != null) return;

        this.lockedLevelMapBtnSpriteSO =
            Resources.Load<ButtonSpriteSO>(
                "UI/BtnSpriteSO/LockedLevelMapBtnSpriteSO"
            );

        Debug.Log(transform.name + ": LoadLockedLevelMapBtnSpriteSO");
    }
    protected void LoadNoStarLevelMapBtnSpriteSO()
    {
        if (this.noStarLevelMapBtnSpriteSO != null) return;
        this.noStarLevelMapBtnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/NoStarLevelMapBtnSpriteSO");
        Debug.Log(transform.name + ": LoadNoStarLevelMapBtnSpriteSO");
    }
    protected void LoadStar1LevelMapBtnSpriteSO()
    {
        if (this.star1LevelMapBtnSpriteSO != null) return;
        this.star1LevelMapBtnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/Star1LevelMapBtnSpriteSO");
        Debug.Log(transform.name + ": LoadStar1LevelMapBtnSpriteSO");
    }
    protected void LoadStar2LevelMapBtnSpriteSO()
    {
        if (this.star2LevelMapBtnSpriteSO != null) return;
        this.star2LevelMapBtnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/Star2LevelMapBtnSpriteSO");
        Debug.Log(transform.name + ": LoadStar2LevelMapBtnSpriteSO");
    }
    protected void LoadStar3LevelMapBtnSpriteSO()
    {
        if (this.star3LevelMapBtnSpriteSO != null) return;
        this.star3LevelMapBtnSpriteSO = Resources.Load<ButtonSpriteSO>("UI/BtnSpriteSO/Star3LevelMapBtnSpriteSO");
        Debug.Log(transform.name + ": LoadStar3LevelMapBtnSpriteSO");
    }
    protected void LoadLevelNumberSpriteUI()
    {
        if (this.levelNumberSpriteUI != null) return;
        this.levelNumberSpriteUI = GetComponentInChildren<LevelNumberSpriteUI>();
        Debug.Log(transform.name + ": LoadLevelNumberSpriteUI");
    }
}
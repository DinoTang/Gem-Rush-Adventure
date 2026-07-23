using UnityEngine;

public class LevelMapBtn : BaseBtn
{
    [SerializeField] protected LevelMapUI levelMapUI;

    protected bool isUnlocked;

    protected override void Start()
    {
        base.Start();
        this.Refresh();
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadLevelMapUI();
    }

    protected void LoadLevelMapUI()
    {
        if (this.levelMapUI != null) return;

        this.levelMapUI = transform.parent.GetComponent<LevelMapUI>();

        Debug.Log(transform.name + ": LoadLevelMapUI");
    }

    protected override void LoadBtnSpriteSO()
    {
        this.LoadBtnSpriteSOInEditor();
    }

    protected void LoadBtnSpriteSOInEditor()
    {
        if (this.btnSpriteSO != null) return;

        this.btnSpriteSO = Resources.Load<ButtonSpriteSO>(
            "UI/BtnSpriteSO/NoStarLevelMapBtnSpriteSO"
        );

        Debug.Log(transform.name + ": LoadBtnSpriteSO", gameObject);
    }

    public void Refresh()
    {
        int levelId = this.levelMapUI.LevelSO.LevelId;

        PlayerSaveData saveData = SaveManager.Instance.SaveData;

        this.isUnlocked = levelId <= saveData.highestUnlockedLevel;

        if (!this.isUnlocked)
        {
            this.ShowLockedState();
            return;
        }

        this.ShowUnlockedState(levelId);
    }

    protected void ShowLockedState()
    {
        this.btnSpriteSO = this.levelMapUI.LockedLevelMapBtnSpriteSO;

        this.buttonImage.sprite = this.btnSpriteSO.Normal;

        // Tuỳ thiết kế:
        // Có thể ẩn số level khi bị khóa.
        this.levelMapUI.LevelNumberSpriteUI.gameObject.SetActive(false);
    }

    protected void ShowUnlockedState(int levelId)
    {
        LevelProgressData levelProgressData = SaveManager.Instance.SaveData.GetLevel(levelId);

        int starCount = levelProgressData?.starCount ?? 0;

        this.btnSpriteSO = this.GetLevelMapBtnSpriteSO(starCount);

        this.buttonImage.sprite = this.btnSpriteSO.Normal;

        // Hiện số level
        this.levelMapUI.LevelNumberSpriteUI.gameObject.SetActive(true);
        this.levelMapUI.LevelNumberSpriteUI.SetNumber(levelId);
    }

    protected override void OnButtonClicked()
    {
        if (!this.isUnlocked) return;

        int levelId = this.levelMapUI.LevelSO.LevelId;

        Debug.Log("Open level: " + levelId);

        // Mở popup level hoặc load gameplay tại đây.
        SceneLoader.Instance.SetLevelSO(this.levelMapUI.LevelSO);
        SceneLoader.Instance.GoToScene(SceneGame.GamePlayScene);
    }

    protected ButtonSpriteSO GetLevelMapBtnSpriteSO(int starCount)
    {
        switch (starCount)
        {
            case 1:
                return this.levelMapUI.Star1LevelMapBtnSpriteSO;

            case 2:
                return this.levelMapUI.Star2LevelMapBtnSpriteSO;

            case 3:
                return this.levelMapUI.Star3LevelMapBtnSpriteSO;

            default:
                return this.levelMapUI.NoStarLevelMapBtnSpriteSO;
        }
    }
}
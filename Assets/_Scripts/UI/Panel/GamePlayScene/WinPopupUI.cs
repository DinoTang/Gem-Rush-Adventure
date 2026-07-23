using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinPopupUI : BaseUI
{
    [SerializeField] protected CoinNumberSpriteUI coinNumberSpriteUI;
    [SerializeField] protected WinScoreNumberSpriteUI winScoreNumberUI;
    [SerializeField] protected WinStarUI1 star1;
    [SerializeField] protected WinStarUI2 star2;
    [SerializeField] protected WinStarUI3 star3;
    [SerializeField] private float starAppearDelay = 0.35f;
    protected bool hasPlayedStar1;
    protected bool hasPlayedStar2;
    protected bool hasPlayedStar3;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadCoinNumberSpriteUI();
        this.LoadWinScoreNumberSpriteUI();
        this.LoadStar1();
        this.LoadStar2();
        this.LoadStar3();
    }
    protected void LoadCoinNumberSpriteUI()
    {
        if (this.coinNumberSpriteUI != null) return;
        this.coinNumberSpriteUI = FindAnyObjectByType<CoinNumberSpriteUI>();
        Debug.Log(transform.name + ": LoadCoinNumberSpriteUI", gameObject);
    }

    protected void LoadWinScoreNumberSpriteUI()
    {
        if (this.winScoreNumberUI != null) return;
        this.winScoreNumberUI = FindAnyObjectByType<WinScoreNumberSpriteUI>();
        Debug.Log(transform.name + ": LoadWinScoreNumberUI", gameObject);
    }
    protected void LoadStar1()
    {
        if (this.star1 != null) return;
        this.star1 = FindAnyObjectByType<WinStarUI1>();
        Debug.Log(transform.name + ": LoadStar1", gameObject);
    }
    protected void LoadStar2()
    {
        if (this.star2 != null) return;
        this.star2 = FindAnyObjectByType<WinStarUI2>();
        Debug.Log(transform.name + ": LoadStar2", gameObject);
    }
    protected void LoadStar3()
    {
        if (this.star3 != null) return;
        this.star3 = FindAnyObjectByType<WinStarUI3>();
        Debug.Log(transform.name + ": LoadStar3", gameObject);
    }
    public override void Show()
    {
        base.Show();

        this.hasPlayedStar1 = false;
        this.hasPlayedStar2 = false;
        this.hasPlayedStar3 = false;

        int finalScore =
            LevelGoalManager.Instance.CurrentScore;

        int earnedCoin =
            LevelGoalManager.Instance.EarnedCoin;

        LevelSO levelData =
            LevelGoalManager.Instance.LevelData;

        int earnedStars =
            this.CalculateStarCount(
                finalScore,
                levelData
            );

        SaveManager.Instance.CompleteLevel(
            levelData.LevelId,
            finalScore,
            earnedStars,
            earnedCoin
        );

        this.winScoreNumberUI.PlayCountAnimation(
            finalScore,
            this.CheckStar,
            this.HandleCompleted
        );

        this.coinNumberSpriteUI.PlayCountAnimation(
            earnedCoin
        );
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.ReloadGame();
            Debug.Log("Reload Game", gameObject);
        }
    }

    public void ReloadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void CheckStar(int score)
    {
        LevelSO levelData =
            LevelGoalManager.Instance.LevelData;

        if (!hasPlayedStar1 &&
            score >= levelData.OneStarScore)
        {
            hasPlayedStar1 = true;

            DOVirtual.DelayedCall(
                0.2f,
                () => star1.PlayUnlockAnimation()
            );
        }

        if (!hasPlayedStar2 &&
            score >= levelData.TwoStarScore)
        {
            hasPlayedStar2 = true;

            DOVirtual.DelayedCall(
                0.5f,
                () => star2.PlayUnlockAnimation()
            );
        }

        if (!hasPlayedStar3 &&
            score >= levelData.ThreeStarScore)
        {
            hasPlayedStar3 = true;

            DOVirtual.DelayedCall(
                0.8f,
                () => star3.PlayUnlockAnimation()
            );
        }
    }

    private int CalculateStarCount(
    int score,
    LevelSO levelData)
    {
        if (score >= levelData.ThreeStarScore)
            return 3;

        if (score >= levelData.TwoStarScore)
            return 2;

        if (score >= levelData.OneStarScore)
            return 1;

        return 0;
    }

    private void HandleCompleted()
    {
        Debug.Log("Score animation completed");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinPopupUI : BaseUI
{
    [SerializeField] protected WinScoreNumberSpriteUI winScoreNumberUI;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadWinScoreNumberUI();
    }

    protected void LoadWinScoreNumberUI()
    {
        if (this.winScoreNumberUI != null) return;
        this.winScoreNumberUI = FindAnyObjectByType<WinScoreNumberSpriteUI>();
        Debug.Log(transform.name + ": LoadWinScoreNumberUI", gameObject);
    }

    public override void Show()
    {
        base.Show();
        int finalScore = LevelGoalManager.Instance.CurrentScore;

        this.winScoreNumberUI.PlayCountAnimation(
            finalScore
        );
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.ReloadGame();
        }
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
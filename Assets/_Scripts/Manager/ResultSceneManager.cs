using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager : BaseBehaviour
{
    [SerializeField] protected WinPopupUI winPopupUI;
    [SerializeField] protected LosePopupUI losePopupUI;
    protected override void Start()
    {
        if (LevelGoalManager.Instance.CurrentLevelState == LevelState.Win)
        {
            this.winPopupUI.Show();
        }
        else if (LevelGoalManager.Instance.CurrentLevelState == LevelState.Lose)
        {
            this.losePopupUI.Show();
        }
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadWinPopupUI();
        this.LosePopupUI();
    }

    protected void LoadWinPopupUI()
    {
        if (this.winPopupUI != null) return;

        this.winPopupUI = FindAnyObjectByType<WinPopupUI>();

        Debug.Log(transform.name + ": LoadWinPopupUI", gameObject);
    }

    protected void LosePopupUI()
    {
        if (this.losePopupUI != null) return;

        this.losePopupUI = FindAnyObjectByType<LosePopupUI>();

        Debug.Log(transform.name + ": LoadLosePopupUI", gameObject);
    }
}
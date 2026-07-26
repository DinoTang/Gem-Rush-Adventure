using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LosePopupUI : BaseUI
{
    [SerializeField] protected FinalScoreNumberSpriteUI finalScoreNumberUI;

    protected override void OnEnable()
    {
        base.OnEnable();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.AudioDataSO.lose);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadFinalScoreNumberSpriteUI();
    }


    protected void LoadFinalScoreNumberSpriteUI()
    {
        if (this.finalScoreNumberUI != null) return;
        this.finalScoreNumberUI = FindAnyObjectByType<FinalScoreNumberSpriteUI>();
        Debug.Log(transform.name + ": LoadFinalScoreNumberSpriteUI", gameObject);
    }

    public override void Show()
    {
        base.Show();

        int finalScore =
            LevelGoalManager.Instance.CurrentScore;

        this.finalScoreNumberUI.PlayCountAnimation(
            finalScore
        );

    }

}
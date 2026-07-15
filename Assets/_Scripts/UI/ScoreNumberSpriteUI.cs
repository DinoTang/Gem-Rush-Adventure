using UnityEngine;

public class ScoreNumberSpriteUI : NumberSpriteUI
{
    protected override void LoadNumberSprites()
    {
        base.LoadNumberSprites();
        this.LoadScoreNumberSprites();
    }
    protected override void Start()
    {
        base.Start();

        LevelGoalManager.Instance.OnScoreChanged += RefreshScore;
        RefreshScore(LevelGoalManager.Instance.CurrentScore);
    }

    protected override void OnDestroy()
    {
        LevelGoalManager.Instance.OnScoreChanged -= RefreshScore;

        base.OnDestroy();
    }

    protected void LoadScoreNumberSprites()
    {
        if (numberSpriteSO != null) return;

        numberSpriteSO = Resources.Load<NumberSpriteSO>("NumberSpriteSO/ScoreNumberSprites");
        Debug.Log(transform.name + ": Load Score Number Sprites", gameObject);
    }

    private void RefreshScore(int value)
    {
        SetNumber(value);
    }
}
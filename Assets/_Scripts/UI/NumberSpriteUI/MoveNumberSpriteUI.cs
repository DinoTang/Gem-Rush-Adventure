using UnityEngine;

public class MoveNumberSpriteUI : NumberSpriteUI
{
    protected override void Start()
    {
        base.Start();

        LevelGoalManager.Instance.OnMoveCountChanged += this.RefreshMoveNumber;

        this.RefreshInitialMoveNumber();
    }

    protected override void OnDestroy()
    {

        LevelGoalManager.Instance.OnMoveCountChanged -= this.RefreshMoveNumber;

        base.OnDestroy();
    }

    protected override void LoadNumberSprites()
    {
        this.LoadMoveNumberSprites();
    }
    protected void LoadMoveNumberSprites()
    {
        if (numberSpriteSO != null) return;

        numberSpriteSO = Resources.Load<NumberSpriteSO>("NumberSpriteSO/MoveNumberSprites");

        Debug.Log(transform.name + ": Load Move Number Sprites", gameObject);
    }
    private void RefreshMoveNumber(int value)
    {
        this.SetNumber(value);
    }

    private void RefreshInitialMoveNumber()
    {
        this.RefreshMoveNumber(LevelGoalManager.Instance.RemainingMoves);
    }
}
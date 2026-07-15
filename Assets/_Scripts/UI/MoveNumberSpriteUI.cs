using UnityEngine;

public class MoveNumberSpriteUI : NumberSpriteUI
{
    protected override void Start()
    {
        base.Start();

        LevelGoalManager.Instance.OnMoveCountChanged += RefreshMoveNumber;

        RefreshInitialMoveNumber();
    }

    protected override void OnDestroy()
    {

        LevelGoalManager.Instance.OnMoveCountChanged -= RefreshMoveNumber;

        base.OnDestroy();
    }

    protected override void LoadNumberSprites()
    {
        if (numberSpriteSO != null)
            return;

        numberSpriteSO = Resources.Load<NumberSpriteSO>(
            "NumberSpriteSO/MoveNumberSprites"
        );

        Debug.Log(
            transform.name + ": Load Move Number Sprites",
            gameObject
        );
    }

    private void RefreshMoveNumber(int value)
    {
        SetNumber(value);
    }

    private void RefreshInitialMoveNumber()
    {
        RefreshMoveNumber(
            LevelGoalManager.Instance.RemainingMoves
        );
    }
}
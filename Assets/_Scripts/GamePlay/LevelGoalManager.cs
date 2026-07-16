using System;
using System.Collections.Generic;
using UnityEngine;
public enum LevelState
{
    Playing,
    Completing,
    WaitingForContinue,
    Win,
    Lose
}

public class LevelGoalManager : BaseBehaviour
{
    public event Action<LevelGoalProgress> OnGoalProgressChanged;
    public event Action<int> OnMoveCountChanged;
    public event Action<int> OnScoreChanged;
    public event Action OnLevelCompleted;
    protected static LevelGoalManager instance;
    public static LevelGoalManager Instance => instance;
    [SerializeField] private LevelData levelData;
    public LevelData LevelData => levelData;

    [SerializeField] private List<LevelGoalProgress> goalProgresses = new();
    public List<LevelGoalProgress> GoalProgresses => goalProgresses;

    [SerializeField] private int remainingMoves;
    public int RemainingMoves => this.remainingMoves;

    [SerializeField] private int currentScore;
    public int CurrentScore => currentScore;
    public LevelState CurrentLevelState { get; private set; } = LevelState.Playing;
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 LevelGoalManager allows to exist");
        instance = this;

        this.InitGoals();
    }

    public void SetLevelState(LevelState state)
    {
        this.CurrentLevelState = state;
    }

    private void InitGoals()
    {
        goalProgresses.Clear();
        this.remainingMoves = this.levelData != null ? this.levelData.moveLimit : 0;

        foreach (LevelGoalData goalData in levelData.goals)
        {
            LevelGoalProgress progress = new(goalData);
            goalProgresses.Add(progress);
            Debug.Log(
            $"{goalData.type} - Target: {goalData.targetAmount}"
        );
        }
    }
    public void UseMove()
    {
        if (this.remainingMoves <= 0)
            return;

        this.remainingMoves--;
        this.OnMoveCountChanged?.Invoke(this.remainingMoves);
    }

    public void AddGemProgress(GemCtrl gemCtrl)
    {
        foreach (LevelGoalProgress progress in this.goalProgresses)
        {
            LevelGoalData data = progress.Data;

            if (data.type != LevelGoalType.CollectGem)
                continue;

            if (data.gemType != gemCtrl.GemData.GemType)
                continue;

            if (data.gemSpecialType != gemCtrl.GemData.GemSpecialType)
                continue;

            progress.DeductProgress();
            OnGoalProgressChanged?.Invoke(progress);
        }

        if (this.AreAllGoalsCompleted())
        {
            OnLevelCompleted?.Invoke();
            Debug.LogWarning("LEVEL COMPLETE");
        }
    }

    private bool AreAllGoalsCompleted()
    {
        foreach (LevelGoalProgress progress in goalProgresses)
        {
            if (!progress.IsCompleted)
                return false;
        }

        return true;
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }
}
using System;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
public enum LevelState
{
    Playing,
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
    public event Action OnLevelFailed;

    protected static LevelGoalManager instance;
    public static LevelGoalManager Instance => instance;
    [SerializeField] private LevelSO levelData;
    public LevelSO LevelData => levelData;

    [SerializeField] private List<LevelGoalProgress> goalProgresses = new();
    public List<LevelGoalProgress> GoalProgresses => goalProgresses;

    [SerializeField] private int remainingMoves;
    public int RemainingMoves => remainingMoves;

    [SerializeField] private int currentScore;
    public int CurrentScore => currentScore;
    [SerializeField] protected int earnedCoin;
    public int EarnedCoin => earnedCoin;
    [SerializeField] public LevelState currentLevelState = LevelState.Playing;
    public LevelState CurrentLevelState => currentLevelState;
    protected bool isCompleted = false;

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        this.InitGoals();
    }

    public void SetLevelState(LevelState state)
    {
        this.currentLevelState = state;
    }

    public void InitializeLevel(LevelSO newLevelData)
    {
        if (newLevelData == null)
        {
            Debug.LogError("LevelGoalManager: LevelData is null!");
            return;
        }

        this.levelData = newLevelData;
        this.InitGoals();
    }

    public void InitGoals()
    {
        // this.levelData = SceneLoader.Instance?.LevelSO;
        this.isCompleted = false;
        this.currentLevelState = LevelState.Playing;
        this.currentScore = 0;
        this.earnedCoin = UnityEngine.Random.Range(10, 21);

        goalProgresses.Clear();
        this.remainingMoves = this.levelData != null ? this.levelData.MoveLimit : 0;

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
        if (this.currentLevelState != LevelState.Playing)
            return;

        if (this.remainingMoves <= 0)
            return;

        this.remainingMoves--;
        this.OnMoveCountChanged?.Invoke(this.remainingMoves);
    }

    public void AddGemProgress(GemCtrl gemCtrl)
    {
        if (this.currentLevelState != LevelState.Playing)
            return;

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
            this.OnGoalProgressChanged?.Invoke(progress);
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

    public void EvaluateLevelResult()
    {
        if (this.currentLevelState != LevelState.Playing)
            return;

        if (this.AreAllGoalsCompleted())
        {
            this.isCompleted = true;
            this.currentLevelState = LevelState.Win;

            Debug.LogWarning("LEVEL COMPLETE", gameObject);
            this.OnLevelCompleted?.Invoke();
            return;
        }

        if (this.remainingMoves < 1)
        {
            this.currentLevelState = LevelState.Lose;

            Debug.LogWarning("LEVEL FAILED", gameObject);
            this.OnLevelFailed?.Invoke();
        }
    }
}
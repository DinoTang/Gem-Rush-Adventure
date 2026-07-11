using System.Collections.Generic;
using UnityEngine;

public class LevelGoalManager : BaseBehaviour
{
    protected static LevelGoalManager instance;
    public static LevelGoalManager Instance => instance;
    [SerializeField] private LevelData levelData;

    [SerializeField] private List<LevelGoalProgress> goalProgresses = new();
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 LevelGoalManager allows to exist");
        instance = this;
    }

    protected override void Start()
    {
        InitGoals();
    }

    private void InitGoals()
    {
        goalProgresses.Clear();

        foreach (LevelGoalData goalData in levelData.goals)
        {
            LevelGoalProgress progress = new(goalData);
            goalProgresses.Add(progress);
            Debug.Log(
            $"{goalData.type} - Target: {goalData.targetAmount}"
        );
        }
    }
    public void AddGemProgress(GemType gemType)
    {
        foreach (LevelGoalProgress progress in goalProgresses)
        {
            LevelGoalData data = progress.Data;

            if (data.type != LevelGoalType.CollectGem)
                continue;

            if (data.gemType != gemType)
                continue;

            progress.AddProgress();
        }

        if (this.AreAllGoalsCompleted())
        {
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
}
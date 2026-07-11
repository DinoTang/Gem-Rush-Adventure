using UnityEngine;

[System.Serializable]
public class LevelGoalProgress
{
    [SerializeField] private LevelGoalData data;
    [SerializeField] private int currentAmount;

    public LevelGoalData Data => data;
    public int CurrentAmount => currentAmount;

    public bool IsCompleted =>
        CurrentAmount >= Data.targetAmount;

    public LevelGoalProgress(LevelGoalData data)
    {
        this.data = data;
        this.currentAmount = 0;
    }

    public void AddProgress(int amount = 1)
    {
        this.currentAmount += amount;

        if (this.currentAmount > Data.targetAmount)
        {
            this.currentAmount = Data.targetAmount;
        }
    }
}
using UnityEngine;

[System.Serializable]
public class LevelGoalProgress
{
    [SerializeField] private LevelGoalData data;
    [SerializeField] private int currentAmount;

    public LevelGoalData Data => data;
    public int CurrentAmount => currentAmount;

    public bool IsCompleted =>
        CurrentAmount <= 0;

    public LevelGoalProgress(LevelGoalData data)
    {
        this.data = data;
        this.currentAmount = data.targetAmount;
    }

    public void DeductProgress(int amount = 1)
    {
        this.currentAmount -= amount;

        if (this.currentAmount < 0)
        {
            this.currentAmount = 0;
        }
    }
}
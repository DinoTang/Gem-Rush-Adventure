using System;

[Serializable]
public class LevelProgressData
{
    public int levelId;
    public int bestScore;
    public int starCount;
    public bool isUnlocked;
    public bool hasClaimedFirstClearReward;
}
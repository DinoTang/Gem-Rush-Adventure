using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int totalStar;
    public int totalCoin;
    public int highestUnlockedLevel = 1;
    public List<LevelProgressData> levels = new();

    public LevelProgressData GetLevel(int levelId)
    {
        return levels.Find(x => x.levelId == levelId);
    }
}

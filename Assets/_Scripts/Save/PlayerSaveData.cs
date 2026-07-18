using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int totalCoin;
    public int highestUnlockedLevel = 1;
    public List<LevelProgressData> levels = new();
}

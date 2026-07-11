using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SO/Level Data")]
public class LevelData : ScriptableObject
{
    public int moveLimit;

    public List<LevelGoalData> goals;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SO/Level Data")]
public class LevelData : ScriptableObject
{
    public int LevelId;
    public int MoveLimit;
    public int OneStarScore;
    public int TwoStarScore;
    public int ThreeStarScore;
    public List<LevelGoalData> goals;
}
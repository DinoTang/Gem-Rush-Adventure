using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO")]
public class LevelSO : ScriptableObject
{
    public int LevelId;
    public int MoveLimit;
    public int OneStarScore;
    public int TwoStarScore;
    public int ThreeStarScore;
    public List<LevelGoalData> goals;
}
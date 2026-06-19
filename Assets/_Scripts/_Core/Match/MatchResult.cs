using System.Collections.Generic;
using UnityEngine;
public enum MatchDirection
{
    Horizontal,
    Vertical
}
public class MatchResult
{
    public List<Vector2Int> Cells = new();
    public MatchDirection MatchDirection;
}

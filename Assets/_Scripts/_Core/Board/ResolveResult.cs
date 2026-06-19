using System;
using System.Collections.Generic;
using UnityEngine;

public class ResolveResult
{
    public List<MatchResult> Matches = new();

    public List<SpecialMergeInfo> MergeInfos = new();

    public List<Vector2Int> CellsToClear = new();

    public HashSet<Vector2Int> ExcludedCells = new();

    public HashSet<Vector2Int> SpecialMergeSourceCells = new();
}
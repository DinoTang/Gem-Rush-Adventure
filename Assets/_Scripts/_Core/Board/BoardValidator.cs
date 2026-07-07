using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardValidator
{

    public bool CanSwap(GemCtrl gemA, GemCtrl gemB)
    {
        Vector2Int posA = gemA.GemData.GridPos;
        Vector2Int posB = gemB.GemData.GridPos;

        if (this.IsAdjacent(posA, posB))
            return true;

        Debug.Log("Swap must be adjacent!");
        return false;
    }

    private bool IsAdjacent(Vector2Int posA, Vector2Int posB)
    {
        int dx = Math.Abs(posA.x - posB.x);
        int dy = Math.Abs(posA.y - posB.y);
        if (dx + dy != 1) return false;

        return true;
    }

    public static void ValidateBoard(BoardManager boardManager, string step)
    {
        if (boardManager == null || boardManager.Grid == null)
            return;

        GridModel<GemCtrl> grid = boardManager.Grid;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GemCtrl gem = grid.Get(x, y);
                Vector2Int cell = new Vector2Int(x, y);

                if (gem == null)
                {
                    Debug.LogWarning(step + " " + cell);
                    continue;
                }

                if (gem.GemData.GridPos != cell)
                {
                    Debug.LogWarning(step + " " + cell + " " + gem.GemData.GridPos);
                }
            }
        }
    }

    public static void ValidateMergeInfos(ResolveResult result)
    {
        if (result.MergeInfos != null && result.MergeInfos.Count > 0)
        {
            HashSet<Vector2Int> duplicateSourceCells = new();
            HashSet<Vector2Int> allSourceCells = new();

            foreach (var info in result.MergeInfos)
            {
                Debug.Log($"MergeInfo SpecialCell={info.SpecialCell}");
                foreach (var source in info.SourceCells)
                {
                    Debug.Log($"MergeInfo SourceCell={source}");
                    if (allSourceCells.Contains(source))
                    {
                        Debug.LogWarning($"MergeInfo duplicate SourceCell found across multiple merge infos: {source}");
                        duplicateSourceCells.Add(source);
                    }
                    else
                    {
                        allSourceCells.Add(source);
                    }
                }
            }
        }
    }
}
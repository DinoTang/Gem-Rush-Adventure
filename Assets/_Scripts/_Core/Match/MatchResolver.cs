using System.Collections.Generic;
using UnityEngine;

public class MatchResolver
{
    private SpecialPatternRegistry specialPatternRegistry = new();
    public List<CellClearInfo> ResolveMatches(
            List<MatchResult> matches,
            GridModel<GemCtrl> grid,
            HashSet<Vector2Int> excluded = null
        )
    {
        HashSet<CellClearInfo> cellsToClear = new();
        HashSet<Vector2Int> visitedCells = new();

        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null && excluded.Contains(cell)) continue;

                if (!visitedCells.Add(cell)) continue;

                cellsToClear.Add(new CellClearInfo
                {
                    GridPos = cell,
                    ClearReason = ClearReason.Match
                });
            }
        }

        return this.ResolveSpecialChains(cellsToClear, visitedCells, grid);
    }

    public List<CellClearInfo> ResolveSpecialChains(
        HashSet<CellClearInfo> inputCells,
        HashSet<Vector2Int> visitedCells,
        GridModel<GemCtrl> grid)
    {
        HashSet<CellClearInfo> cellsToClear = new(inputCells);

        Queue<Vector2Int> specialQueue = new();
        foreach (var info in inputCells)
        {
            GemCtrl gem = grid.Get(info.GridPos.x, info.GridPos.y);

            if (gem == null) continue;

            if (gem.GemData.GemSpecialType != GemSpecialType.None)
                specialQueue.Enqueue(info.GridPos);
        }
        while (specialQueue.Count > 0)
        {
            var specialCell = specialQueue.Dequeue();

            GemCtrl specialGem = grid.Get(specialCell.x, specialCell.y);
            if (specialGem == null) continue;

            var extraCells =
            this.specialPatternRegistry
            .GetPattern(specialGem.GemData.GemSpecialType)
            .GetCells(specialGem, grid);

            ClearReason reason = this.ConvertReason(specialGem.GemData.GemSpecialType);

            foreach (var cell in extraCells)
            {
                if (!visitedCells.Add(cell))
                    continue;

                cellsToClear.Add(new CellClearInfo
                {
                    GridPos = cell,
                    ClearReason = reason
                });

                GemCtrl gem = grid.Get(cell.x, cell.y);
                if (gem == null) continue;

                if (gem.GemData.GemSpecialType != GemSpecialType.None)
                    specialQueue.Enqueue(cell);
            }
        }
        return new List<CellClearInfo>(cellsToClear);
    }

    public ClearReason ConvertReason(
    GemSpecialType specialType)
    {
        return specialType switch
        {
            GemSpecialType.HorizontalRocket => ClearReason.Rocket,
            GemSpecialType.VerticalRocket => ClearReason.Rocket,
            GemSpecialType.Bomb => ClearReason.Bomb,
            GemSpecialType.Cube => ClearReason.Cube,

            _ => ClearReason.Match
        };
    }
}
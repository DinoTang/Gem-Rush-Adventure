using System.Collections.Generic;
using UnityEngine;

public class MatchResolver
{
    // private SpecialResolver specialResolver = new();
    private SpecialPatternRegistry specialPatternRegistry = new();
    public List<Vector2Int> ResolveMatches(
            List<MatchResult> matches,
            GridModel<GemCtrl> grid,
            HashSet<Vector2Int> excluded = null
        )
    {
        HashSet<Vector2Int> cellsToClear = new();


        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null && excluded.Contains(cell)) continue;

                cellsToClear.Add(cell);
            }
        }

        return this.ResolveSpecialChains(new List<Vector2Int>(cellsToClear), grid);
    }

    public List<Vector2Int> ResolveSpecialChains(List<Vector2Int> inputCells, GridModel<GemCtrl> grid)
    {
        HashSet<Vector2Int> cellsToClear = new(inputCells);

        Queue<Vector2Int> specialQueue = new();
        foreach (var cell in inputCells)
        {
            GemCtrl gem = grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            if (gem.GemData.GemSpecialType != GemSpecialType.None)
                specialQueue.Enqueue(cell);
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
            
            Debug.LogWarning("So luong gem bi clear boi cube gem: " + extraCells.Count);
            VFXSpawner.Instance.SpawnSpecialVFX(specialGem);

            if (specialGem.GemData.GemSpecialType == GemSpecialType.Cube)
            {
                VFXSpawner.Instance.SpawnCubeLightningVFX(specialGem, extraCells, grid);
                Debug.LogWarning($"Cube lightning VFX spawned for cube gem at {specialCell}");
            }

            foreach (var cell in extraCells)
            {
                if (cellsToClear.Contains(cell)) continue;

                cellsToClear.Add(cell);

                GemCtrl gem = grid.Get(cell.x, cell.y);
                if (gem == null) continue;

                if (gem.GemData.GemSpecialType != GemSpecialType.None)
                    specialQueue.Enqueue(cell);
            }
        }
        return new List<Vector2Int>(cellsToClear);
    }
}
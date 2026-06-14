using System.Collections.Generic;

public class MatchResolver
{
    // private SpecialResolver specialResolver = new();
    private SpecialPatternRegistry specialPatternRegistry = new();
    public List<(int x, int y)> ResolveMatches(
            List<MatchResult> matches,
            GridModel<GemCtrl> grid,
            List<(int x, int y)> excluded = null
        )
    {
        HashSet<(int x, int y)> cellsToClear = new();


        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null && excluded.Contains(cell)) continue;

                cellsToClear.Add(cell);
            }
        }

        return this.ResolveSpecialChains(new List<(int x, int y)>(cellsToClear), grid);
    }

    public List<(int x, int y)> ResolveSpecialChains(List<(int x, int y)> inputCells, GridModel<GemCtrl> grid)
    {
        HashSet<(int x, int y)> cellsToClear = new(inputCells);

        Queue<(int x, int y)> specialQueue = new();
        foreach (var cell in inputCells)
        {
            GemCtrl gem = grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            if (gem.GemModel.GemSpecialType != GemSpecialType.None)
                specialQueue.Enqueue(cell);
        }

        while (specialQueue.Count > 0)
        {
            var specialCell = specialQueue.Dequeue();

            GemCtrl specialGem = grid.Get(specialCell.x, specialCell.y);
            if (specialGem == null) continue;

            var extraCells =
            this.specialPatternRegistry
            .GetPattern(specialGem.GemModel.GemSpecialType)
            .GetCells(specialGem, grid);

            VFXSpawner.Instance.SpawnSpecialVFX(specialGem);

            foreach (var cell in extraCells)
            {
                if (cellsToClear.Contains(cell)) continue;

                cellsToClear.Add(cell);

                GemCtrl gem = grid.Get(cell.x, cell.y);
                if (gem == null) continue;

                if (gem.GemModel.GemSpecialType != GemSpecialType.None)
                    specialQueue.Enqueue(cell);
            }
        }
        return new List<(int x, int y)>(cellsToClear);
    }
}
using System.Collections.Generic;

public class MatchResolver
{
    private SpecialResolver specialResolver = new();

    public List<(int x, int y)> ResolveMatches(
            List<MatchResult> matches,
            GridModel<GemCtrl> grid,
            List<(int x, int y)> excluded = null
        )
    {
        HashSet<(int x, int y)> cellsToClear = new();

        Queue<GemCtrl> specialQueue = new();

        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                if (excluded != null && excluded.Contains(cell)) continue;
                cellsToClear.Add(cell);

                GemCtrl gem = grid.Get(cell.x, cell.y);

                if (gem == null) continue;

                if (gem.GemModel.GemSpecialType != GemSpecialType.None)
                    specialQueue.Enqueue(gem);
            }
        }

        while (specialQueue.Count > 0)
        {
            GemCtrl specialGem = specialQueue.Dequeue();
            var extraCells = specialResolver.Resolve(specialGem, grid);

            foreach (var cell in extraCells)
            {
                if (cellsToClear.Contains(cell)) continue;

                cellsToClear.Add(cell);

                GemCtrl gem = grid.Get(cell.x, cell.y);
                if (gem == null) continue;

                if (gem.GemModel.GemSpecialType != GemSpecialType.None)
                    specialQueue.Enqueue(gem);
            }
        }


        return new List<(int x, int y)>(cellsToClear);
    }


}
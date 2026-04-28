using System.Collections.Generic;

public class MatchResolver
{
    public List<(int x, int y)> GetCellsToClear(List<MatchResult> matches, List<(int x, int y)> excluded = null)
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

        return new List<(int x, int y)>(cellsToClear);
    }

    public void ClearMatches(List<MatchResult> matches, GridModel<GemCtrl> grid, List<(int x, int y)> excluded = null)
    {
        List<(int x, int y)> cells = GetCellsToClear(matches, excluded);

        foreach (var cell in cells)
        {
            var gem = grid.Get(cell.x, cell.y);

            if (gem == null) continue;

            gem.GemDespawn.DoDespawn();
            grid.Set(cell.x, cell.y, null);
        }
    }
}
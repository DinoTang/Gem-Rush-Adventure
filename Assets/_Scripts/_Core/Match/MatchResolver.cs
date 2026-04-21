using System.Collections.Generic;

public class MatchResolver
{
    public List<(int x, int y)> GetCellsToClear(List<MatchResult> matches)
    {
        HashSet<(int x, int y)> cellsToClear = new();

        foreach (var match in matches)
        {
            foreach (var cell in match.Cells)
            {
                cellsToClear.Add(cell);
            }
        }

        return new List<(int x, int y)>(cellsToClear);
    }

    public void ClearMatches(
        List<MatchResult> matches,
        GridModel<GemCtrl> grid,
        GemSpawner gemSpawner)
    {
        List<(int x, int y)> cells = GetCellsToClear(matches);

        foreach (var cell in cells)
        {
            GemCtrl gem = grid.Get(cell.x, cell.y);

            if (gem == null)
                continue;

            gemSpawner.Despawn(gem);
            grid.Set(cell.x, cell.y, null);
        }
    }
}
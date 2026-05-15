using System.Collections.Generic;

public class RowRocketPattern : ISpecialPattern
{
    public List<(int x, int y)> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<(int x, int y)> cells = new();
        int row = gem.GridPos.y;

        for (int x = 0; x < grid.Width; x++)
        {
            GemCtrl target = grid.Get(x, row);
            if (target == null) continue;

            cells.Add((x, row));
        }
        return cells;
    }
}
using System.Collections.Generic;

public class ColumnRocketPattern : ISpecialPattern
{
    public List<(int x, int y)> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<(int x, int y)> cells = new();
        int colum = gem.GridPos.x;

        for (int y = 0; y < grid.Height; y++)
        {
            GemCtrl target = grid.Get(colum, y);
            if (target == null) continue;

            cells.Add((colum, y));
        }
        return cells;
    }
}
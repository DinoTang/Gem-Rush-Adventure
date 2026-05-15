using System.Collections.Generic;

public class CubePattern : ISpecialPattern
{
    public List<(int x, int y)> GetCells(GemCtrl target, GridModel<GemCtrl> grid)
    {
        List<(int x, int y)> cells = new();

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                GemCtrl sameTarget = grid.Get(x, y);
                if (sameTarget == null) continue;
                if (sameTarget.GemModel.GemType != target.GemModel.GemType) continue;

                cells.Add((x, y));
            }
        }
        return cells;
    }
}
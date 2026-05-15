using System.Collections.Generic;

public class BombPattern : ISpecialPattern
{
    public List<(int x, int y)> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<(int x, int y)> cells = new();

        int centerX = gem.GridPos.x;
        int centerY = gem.GridPos.y;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                int targetX = x + centerX;
                int targetY = y + centerY;

                if (!grid.IsInBounds(targetX, targetY)) continue;

                GemCtrl target = grid.Get(targetX, targetY);
                if (target == null) continue;

                cells.Add((targetX, targetY));
            }
        }
        return cells;
    }
}
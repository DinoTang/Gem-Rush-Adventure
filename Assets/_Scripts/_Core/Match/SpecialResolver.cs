using System.Collections.Generic;

public class SpecialResolver
{
    public List<(int x, int y)> Resolve(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        switch (gem.GemModel.GemSpecialType)
        {
            case GemSpecialType.HorizontalRocket:
                return GetRowCells(gem, grid);

            case GemSpecialType.VerticalRocket:
                return GetColumnCells(gem, grid);

            case GemSpecialType.Bomb:
                return GetBombCells(gem, grid);

            default:
                return new();
        }
    }

    public List<(int x, int y)> GetRowCells(GemCtrl gem, GridModel<GemCtrl> grid)
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

    public List<(int x, int y)> GetColumnCells(GemCtrl gem, GridModel<GemCtrl> grid)
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
    public List<(int x, int y)> GetBombCells(GemCtrl gem, GridModel<GemCtrl> grid)
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

    public List<(int x, int y)> GetCubeCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<(int x, int y)> cells = new();

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                GemCtrl target = grid.Get(x, y);
                if (target == null) continue;
                if (target.GemModel.GemType != gem.GemModel.GemType) continue;

                cells.Add((x, y));
            }
        }
        return cells;
    }
}
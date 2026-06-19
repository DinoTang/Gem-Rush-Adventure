using System.Collections.Generic;
using UnityEngine;

public class BombPattern : ISpecialPattern
{
    public List<Vector2Int> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<Vector2Int> cells = new();

        int centerX = gem.GemData.GridPos.x;
        int centerY = gem.GemData.GridPos.y;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                int targetX = x + centerX;
                int targetY = y + centerY;

                if (!grid.IsInBounds(targetX, targetY)) continue;

                GemCtrl target = grid.Get(targetX, targetY);
                if (target == null) continue;

                cells.Add(new Vector2Int(targetX, targetY));
            }
        }
        return cells;
    }
}
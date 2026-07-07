using System.Collections.Generic;
using UnityEngine;

public class RowRocketPattern : ISpecialPattern
{
    public List<Vector2Int> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<Vector2Int> cells = new();
        int row = gem.GemData.GridPos.y;

        for (int x = 0; x < grid.Width; x++)
        {
            GemCtrl target = grid.Get(x, row);
            if (target == null) continue;

            cells.Add(new Vector2Int(x, row));
            target.GemData.SetClearReason(ClearReason.Rocket);
        }
        return cells;
    }
}
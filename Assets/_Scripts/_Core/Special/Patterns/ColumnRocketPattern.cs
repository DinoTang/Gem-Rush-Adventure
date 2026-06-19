using System.Collections.Generic;
using UnityEngine;

public class ColumnRocketPattern : ISpecialPattern
{
    public List<Vector2Int> GetCells(GemCtrl gem, GridModel<GemCtrl> grid)
    {
        List<Vector2Int> cells = new();
        int colum = gem.GemData.GridPos.x;

        for (int y = 0; y < grid.Height; y++)
        {
            GemCtrl target = grid.Get(colum, y);
            if (target == null) continue;

            cells.Add(new Vector2Int(colum, y));
        }
        return cells;
    }
}